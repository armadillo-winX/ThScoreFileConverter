﻿//-----------------------------------------------------------------------
// <copyright file="Th125Converter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591
#pragma warning disable SA1600 // ElementsMustBeDocumented

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Extensions;
using ThScoreFileConverter.Models.Th125;

namespace ThScoreFileConverter.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reviewed.")]
    internal class Th125Converter : ThConverter
    {
        private AllScoreData allScoreData = null;

        private Dictionary<
            (Chara, Th125.Level Level, int Scene), (string Path, IBestShotHeader Header)> bestshots = null;

        public override string SupportedVersions
        {
            get { return "1.00a"; }
        }

        public override bool HasBestShotConverter
        {
            get { return true; }
        }

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th125decoded.dat", FileMode.Create, FileAccess.ReadWrite))
#else
            using (var decoded = new MemoryStream())
#endif
            {
                if (!Decrypt(input, decrypted))
                    return false;

                decrypted.Seek(0, SeekOrigin.Begin);
                if (!Extract(decrypted, decoded))
                    return false;

                decoded.Seek(0, SeekOrigin.Begin);
                if (!Validate(decoded))
                    return false;

                decoded.Seek(0, SeekOrigin.Begin);
                this.allScoreData = Read(decoded);

                return this.allScoreData != null;
            }
        }

        protected override IEnumerable<IStringReplaceable> CreateReplacers(
            bool hideUntriedCards, string outputFilePath)
        {
            return new List<IStringReplaceable>
            {
                new ScoreReplacer(this.allScoreData.Scores),
                new ScoreTotalReplacer(this.allScoreData.Scores),
                new CardReplacer(this.allScoreData.Scores, hideUntriedCards),
                new TimeReplacer(this.allScoreData.Status),
                new ShotReplacer(this.bestshots, outputFilePath),
                new ShotExReplacer(this.bestshots, this.allScoreData.Scores, outputFilePath),
            };
        }

        protected override string[] FilterBestShotFiles(string[] files)
        {
            var pattern = Utils.Format(@"bs2?_({0})_[1-9].dat", Parsers.LevelLongPattern);

            return files.Where(file => Regex.IsMatch(
                Path.GetFileName(file), pattern, RegexOptions.IgnoreCase)).ToArray();
        }

        protected override void ConvertBestShot(Stream input, Stream output)
        {
            using (var decoded = new MemoryStream())
            {
                var outputFile = output as FileStream;
                var chara = Path.GetFileName(outputFile.Name)
                    .StartsWith("bs2_", StringComparison.CurrentCultureIgnoreCase)
                    ? Chara.Hatate : Chara.Aya;

                using (var reader = new BinaryReader(input, Encoding.UTF8, true))
                {
                    var header = new BestShotHeader();
                    header.ReadFrom(reader);

                    if (this.bestshots == null)
                    {
                        this.bestshots =
                            new Dictionary<(Chara, Th125.Level, int), (string, IBestShotHeader)>(
                                Enum.GetValues(typeof(Chara)).Length * Definitions.SpellCards.Count);
                    }

                    var key = (chara, header.Level, header.Scene);
                    if (!this.bestshots.ContainsKey(key))
                        this.bestshots.Add(key, (outputFile.Name, header));

                    Lzss.Extract(input, decoded);

                    decoded.Seek(0, SeekOrigin.Begin);
                    using (var bitmap = new Bitmap(header.Width, header.Height, PixelFormat.Format32bppArgb))
                    {
                        try
                        {
                            var permission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                            permission.Demand();

                            var bitmapData = bitmap.LockBits(
                                new Rectangle(0, 0, header.Width, header.Height),
                                ImageLockMode.WriteOnly,
                                bitmap.PixelFormat);
                            var source = decoded.ToArray();
                            var destination = bitmapData.Scan0;
                            Marshal.Copy(source, 0, destination, source.Length);
                            bitmap.UnlockBits(bitmapData);
                        }
                        catch (SecurityException e)
                        {
                            Console.WriteLine(e.ToString());
                        }

                        bitmap.Save(output, ImageFormat.Png);
                        output.Flush();
                        output.SetLength(output.Position);
                    }
                }
            }
        }

        private static bool Decrypt(Stream input, Stream output)
        {
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            using (var writer = new BinaryWriter(output, Encoding.UTF8, true))
            {
                var header = new Header();
                header.ReadFrom(reader);
                if (!header.IsValid)
                    return false;
                if (header.EncodedAllSize != reader.BaseStream.Length)
                    return false;

                header.WriteTo(writer);
                ThCrypt.Decrypt(input, output, header.EncodedBodySize, 0xAC, 0x35, 0x10, header.EncodedBodySize);

                return true;
            }
        }

        private static bool Extract(Stream input, Stream output)
        {
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            using (var writer = new BinaryWriter(output, Encoding.UTF8, true))
            {
                var header = new Header();
                header.ReadFrom(reader);
                header.WriteTo(writer);

                var bodyBeginPos = output.Position;
                Lzss.Extract(input, output);
                output.Flush();
                output.SetLength(output.Position);

                return header.DecodedBodySize == (output.Position - bodyBeginPos);
            }
        }

        private static bool Validate(Stream input)
        {
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                var header = new Header();
                header.ReadFrom(reader);
                var remainSize = header.DecodedBodySize;
                var chapter = new Th095.Chapter();

                try
                {
                    while (remainSize > 0)
                    {
                        chapter.ReadFrom(reader);
                        if (!chapter.IsValid)
                            return false;
                        if (!Score.CanInitialize(chapter) && !Status.CanInitialize(chapter))
                            return false;

                        remainSize -= chapter.Size;
                    }
                }
                catch (EndOfStreamException)
                {
                    // It's OK, do nothing.
                }

                return remainSize == 0;
            }
        }

        private static AllScoreData Read(Stream input)
        {
            var dictionary = new Dictionary<string, Action<AllScoreData, Th095.Chapter>>
            {
                { Score.ValidSignature,  (data, ch) => data.Set(new Score(ch))  },
                { Status.ValidSignature, (data, ch) => data.Set(new Status(ch)) },
            };

            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                var allScoreData = new AllScoreData();
                var chapter = new Th095.Chapter();

                var header = new Header();
                header.ReadFrom(reader);
                allScoreData.Set(header);

                try
                {
                    while (true)
                    {
                        chapter.ReadFrom(reader);
                        if (dictionary.TryGetValue(chapter.Signature, out var setChapter))
                            setChapter(allScoreData, chapter);
                    }
                }
                catch (EndOfStreamException)
                {
                    // It's OK, do nothing.
                }

                if ((allScoreData.Header != null) &&
                    //// (allScoreData.scores.Count >= 0) &&
                    (allScoreData.Status != null))
                    return allScoreData;
                else
                    return null;
            }
        }

        // %T125SCR[w][x][y][z]
        private class ScoreReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T125SCR({0})({1})([1-9])([1-5])", Parsers.CharaParser.Pattern, Parsers.LevelParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ScoreReplacer(IReadOnlyList<IScore> scores)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var chara = Parsers.CharaParser.Parse(match.Groups[1].Value);
                    var level = Parsers.LevelParser.Parse(match.Groups[2].Value);
                    var scene = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    var key = (level, scene);
                    if (!Definitions.SpellCards.ContainsKey(key))
                        return match.ToString();

                    var score = scores.FirstOrDefault(elem =>
                        (elem != null) && (elem.Chara == chara) && elem.LevelScene.Equals(key));

                    switch (type)
                    {
                        case 1:     // high score
                            return (score != null) ? Utils.ToNumberString(score.HighScore) : "0";
                        case 2:     // bestshot score
                            return (score != null) ? Utils.ToNumberString(score.BestshotScore) : "0";
                        case 3:     // num of shots
                            return (score != null) ? Utils.ToNumberString(score.TrialCount) : "0";
                        case 4:     // num of shots for the first success
                            return (score != null) ? Utils.ToNumberString(score.FirstSuccess) : "0";
                        case 5:     // date & time
                            return (score != null)
                                ? new DateTime(1970, 1, 1).AddSeconds(score.DateTime).ToLocalTime()
                                    .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture)
                                : "----/--/-- --:--:--";
                        default:    // unreachable
                            return match.ToString();
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T125SCRTL[x][y][z]
        private class ScoreTotalReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T125SCRTL({0})([12])([1-5])", Parsers.CharaParser.Pattern);

            private static readonly Func<IScore, Chara, int, bool> IsTargetImpl =
                (score, chara, method) =>
                {
                    if (score == null)
                        return false;

                    if (method == 1)
                    {
                        if (score.LevelScene.Level == Th125.Level.Spoiler)
                        {
                            if (chara == Chara.Aya)
                            {
                                if (score.LevelScene.Scene <= 4)
                                    return score.Chara == Chara.Aya;
                                else
                                    return score.Chara == Chara.Hatate;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return score.Chara == chara;
                        }
                    }
                    else
                    {
                        return score.Chara == chara;
                    }
                };

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public ScoreTotalReplacer(IReadOnlyList<IScore> scores)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var chara = Parsers.CharaParser.Parse(match.Groups[1].Value);
                    var method = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    Func<IScore, bool> isTarget = (score => IsTargetImpl(score, chara, method));
                    Func<IScore, bool> triedAndSucceeded =
                        (score => isTarget(score) && (score.TrialCount > 0) && (score.FirstSuccess > 0));

                    switch (type)
                    {
                        case 1:     // total score
                            return Utils.ToNumberString(
                                scores.Sum(score => triedAndSucceeded(score) ? score.HighScore : 0L));
                        case 2:     // total of bestshot scores
                            return Utils.ToNumberString(
                                scores.Sum(score => isTarget(score) ? score.BestshotScore : 0L));
                        case 3:     // total of num of shots
                            return Utils.ToNumberString(
                                scores.Sum(score => isTarget(score) ? score.TrialCount : 0));
                        case 4:     // total of num of shots for the first success
                            return Utils.ToNumberString(
                                scores.Sum(score => triedAndSucceeded(score) ? score.FirstSuccess : 0L));
                        case 5:     // num of succeeded scenes
                            return scores.Count(triedAndSucceeded).ToString(CultureInfo.CurrentCulture);
                        default:    // unreachable
                            return match.ToString();
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T125CARD[x][y][z]
        private class CardReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T125CARD({0})([1-9])([12])", Parsers.LevelParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public CardReplacer(IReadOnlyList<IScore> scores, bool hideUntriedCards)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = Parsers.LevelParser.Parse(match.Groups[1].Value);
                    var scene = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    var key = (level, scene);
                    if (!Definitions.SpellCards.ContainsKey(key))
                        return match.ToString();

                    if (hideUntriedCards)
                    {
                        var score = scores.FirstOrDefault(elem => (elem != null) && elem.LevelScene.Equals(key));
                        if (score == null)
                            return "??????????";
                    }

                    return (type == 1)
                        ? Definitions.SpellCards[key].Enemy.ToLongName() : Definitions.SpellCards[key].Card;
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T125TIMEPLY
        private class TimeReplacer : IStringReplaceable
        {
            private const string Pattern = @"%T125TIMEPLY";

            private readonly MatchEvaluator evaluator;

            public TimeReplacer(IStatus status)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    return new Time(status.TotalPlayTime * 10, false).ToLongString();
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T125SHOT[x][y][z]
        private class ShotReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T125SHOT({0})({1})([1-9])", Parsers.CharaParser.Pattern, Parsers.LevelParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ShotReplacer(
                IReadOnlyDictionary<(Chara, Th125.Level, int), (string Path, IBestShotHeader Header)> bestshots,
                string outputFilePath)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var chara = Parsers.CharaParser.Parse(match.Groups[1].Value);
                    var level = Parsers.LevelParser.Parse(match.Groups[2].Value);
                    var scene = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    if (!Definitions.SpellCards.ContainsKey((level, scene)))
                        return match.ToString();

                    if (!string.IsNullOrEmpty(outputFilePath) &&
                        bestshots.TryGetValue((chara, level, scene), out var bestshot))
                    {
                        var relativePath = new Uri(outputFilePath)
                            .MakeRelativeUri(new Uri(bestshot.Path)).OriginalString;
                        var alternativeString = Utils.Format(
                            "ClearData: {0}{3}Slow: {1:F6}%{3}SpellName: {2}",
                            Utils.ToNumberString(bestshot.Header.ResultScore),
                            bestshot.Header.SlowRate,
                            Encoding.Default.GetString(bestshot.Header.CardName.ToArray()).TrimEnd('\0'),
                            Environment.NewLine);
                        return Utils.Format(
                            "<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" border=0>",
                            relativePath,
                            alternativeString);
                    }
                    else
                    {
                        return string.Empty;
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T125SHOTEX[w][x][y][z]
        private class ShotExReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T125SHOTEX({0})({1})([1-9])([1-7])", Parsers.CharaParser.Pattern, Parsers.LevelParser.Pattern);

            private static readonly Func<IBestShotHeader, List<Detail>> DetailList =
                header => new List<Detail>
                {
                    new Detail(true,                       "Base Point    {0,9}", Utils.ToNumberString(header.BasePoint)),
                    new Detail(header.Fields.ClearShot,    "Clear Shot!   {0,9}", Utils.Format("+ {0}", header.ClearShot)),
                    new Detail(header.Fields.SoloShot,     "Solo Shot     {0,9}", "+ 100"),
                    new Detail(header.Fields.RedShot,      "Red Shot      {0,9}", "+ 300"),
                    new Detail(header.Fields.PurpleShot,   "Purple Shot   {0,9}", "+ 300"),
                    new Detail(header.Fields.BlueShot,     "Blue Shot     {0,9}", "+ 300"),
                    new Detail(header.Fields.CyanShot,     "Cyan Shot     {0,9}", "+ 300"),
                    new Detail(header.Fields.GreenShot,    "Green Shot    {0,9}", "+ 300"),
                    new Detail(header.Fields.YellowShot,   "Yellow Shot   {0,9}", "+ 300"),
                    new Detail(header.Fields.OrangeShot,   "Orange Shot   {0,9}", "+ 300"),
                    new Detail(header.Fields.ColorfulShot, "Colorful Shot {0,9}", "+ 900"),
                    new Detail(header.Fields.RainbowShot,  "Rainbow Shot  {0,9}", Utils.Format("+ {0}", Utils.ToNumberString(2100))),
                    new Detail(header.Fields.RiskBonus,    "Risk Bonus    {0,9}", Utils.Format("+ {0}", Utils.ToNumberString(header.RiskBonus))),
                    new Detail(header.Fields.MacroBonus,   "Macro Bonus   {0,9}", Utils.Format("+ {0}", Utils.ToNumberString(header.MacroBonus))),
                    new Detail(header.Fields.FrontShot,    "Front Shot    {0,9}", Utils.Format("+ {0}", header.FrontSideBackShot)),
                    new Detail(header.Fields.SideShot,     "Side Shot     {0,9}", Utils.Format("+ {0}", header.FrontSideBackShot)),
                    new Detail(header.Fields.BackShot,     "Back Shot     {0,9}", Utils.Format("+ {0}", header.FrontSideBackShot)),
                    new Detail(header.Fields.CatBonus,     "Cat Bonus     {0,9}", "+ 666"),
                    new Detail(true,                       string.Empty,          string.Empty),
                    new Detail(true,                       "Boss Shot!    {0,9}", Utils.Format("* {0:F2}", header.BossShot)),
                    new Detail(header.Fields.TwoShot,      "Two Shot!     {0,9}", "* 1.50"),
                    new Detail(header.Fields.NiceShot,     "Nice Shot!    {0,9}", Utils.Format("* {0:F2}", header.NiceShot)),
                    new Detail(true,                       "Angle Bonus   {0,9}", Utils.Format("* {0:F2}", header.AngleBonus)),
                    new Detail(true,                       string.Empty,          string.Empty),
                    new Detail(true,                       "Result Score  {0,9}", Utils.ToNumberString(header.ResultScore)),
                };

            private readonly MatchEvaluator evaluator;

            public ShotExReplacer(
                IReadOnlyDictionary<(Chara, Th125.Level, int), (string Path, IBestShotHeader Header)> bestshots,
                IReadOnlyList<IScore> scores,
                string outputFilePath)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var chara = Parsers.CharaParser.Parse(match.Groups[1].Value);
                    var level = Parsers.LevelParser.Parse(match.Groups[2].Value);
                    var scene = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    var key = (level, scene);
                    if (!Definitions.SpellCards.ContainsKey(key))
                        return match.ToString();

                    if (!string.IsNullOrEmpty(outputFilePath) &&
                        bestshots.TryGetValue((chara, level, scene), out var bestshot))
                    {
                        IScore score;
                        IEnumerable<string> detailStrings;
                        switch (type)
                        {
                            case 1:     // relative path to the bestshot file
                                return new Uri(outputFilePath)
                                    .MakeRelativeUri(new Uri(bestshot.Path)).OriginalString;
                            case 2:     // width
                                return bestshot.Header.Width.ToString(CultureInfo.InvariantCulture);
                            case 3:     // height
                                return bestshot.Header.Height.ToString(CultureInfo.InvariantCulture);
                            case 4:     // score
                                return Utils.ToNumberString(bestshot.Header.ResultScore);
                            case 5:     // slow rate
                                return Utils.Format("{0:F6}%", bestshot.Header.SlowRate);
                            case 6:     // date & time
                                score = scores.FirstOrDefault(elem =>
                                    (elem != null) && (elem.Chara == chara) && elem.LevelScene.Equals(key));
                                if (score == null)
                                    return "----/--/-- --:--:--";
                                return new DateTime(1970, 1, 1)
                                    .AddSeconds(score.DateTime).ToLocalTime()
                                    .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture);
                            case 7:     // detail info
                                detailStrings = DetailList(bestshot.Header)
                                    .Where(detail => detail.Outputs)
                                    .Select(detail => Utils.Format(detail.Format, detail.Value));
                                return string.Join(Environment.NewLine, detailStrings.ToArray());
                            default:    // unreachable
                                return match.ToString();
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case 1: return string.Empty;
                            case 2: return "0";
                            case 3: return "0";
                            case 4: return "--------";
                            case 5: return "-----%";
                            case 6: return "----/--/-- --:--:--";
                            case 7: return string.Empty;
                            default: return match.ToString();
                        }
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        private class Detail
        {
            public Detail(bool outputs, string format, string value)
            {
                this.Outputs = outputs;
                this.Format = format;
                this.Value = value;
            }

            public bool Outputs { get; }

            public string Format { get; }

            public string Value { get; }
        }
    }
}
