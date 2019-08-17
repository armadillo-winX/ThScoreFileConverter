﻿//-----------------------------------------------------------------------
// <copyright file="Th09Converter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591
#pragma warning disable SA1600 // ElementsMustBeDocumented
#pragma warning disable SA1602 // EnumerationItemsMustBeDocumented

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Models.Th09;

namespace ThScoreFileConverter.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reviewed.")]
    internal class Th09Converter : ThConverter
    {
        private static readonly EnumShortNameParser<Chara> CharaParser = new EnumShortNameParser<Chara>();

        private AllScoreData allScoreData = null;

        public enum Chara
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("RM")] Reimu,
            [EnumAltName("MR")] Marisa,
            [EnumAltName("SK")] Sakuya,
            [EnumAltName("YM")] Youmu,
            [EnumAltName("RS")] Reisen,
            [EnumAltName("CI")] Cirno,
            [EnumAltName("LY")] Lyrica,
            [EnumAltName("MY")] Mystia,
            [EnumAltName("TW")] Tewi,
            [EnumAltName("YU")] Yuuka,
            [EnumAltName("AY")] Aya,
            [EnumAltName("MD")] Medicine,
            [EnumAltName("KM")] Komachi,
            [EnumAltName("SI")] Shikieiki,
            [EnumAltName("ML")] Merlin,
            [EnumAltName("LN")] Lunasa,
#pragma warning restore SA1134 // Attributes should not share line
        }

        public override string SupportedVersions
        {
            get { return "1.50a"; }
        }

        public override bool HasCardReplacer
        {
            get { return false; }
        }

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th09decoded.dat", FileMode.Create, FileAccess.ReadWrite))
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
                new ScoreReplacer(this),
                new TimeReplacer(this),
                new ClearReplacer(this),
            };
        }

        private static bool Decrypt(Stream input, Stream output)
        {
            var size = (int)input.Length;
            ThCrypt.Decrypt(input, output, size, 0x3A, 0xCD, 0x0100, 0x0C00);

            var data = new byte[size];
            output.Seek(0, SeekOrigin.Begin);
            output.Read(data, 0, size);

            uint checksum = 0;
            byte temp = 0;
            for (var index = 2; index < size; index++)
            {
                temp += data[index - 1];
                temp = (byte)((temp >> 5) | (temp << 3));
                data[index] ^= temp;
                if (index > 3)
                    checksum += data[index];
            }

            output.Seek(0, SeekOrigin.Begin);
            output.Write(data, 0, size);

            return (ushort)checksum == BitConverter.ToUInt16(data, 2);
        }

        private static bool Extract(Stream input, Stream output)
        {
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            using (var writer = new BinaryWriter(output, Encoding.UTF8, true))
            {
                var header = new FileHeader();

                header.ReadFrom(reader);
                if (!header.IsValid)
                    return false;
                if (header.Size + header.EncodedBodySize != input.Length)
                    return false;

                header.WriteTo(writer);

                Lzss.Extract(input, output);
                output.Flush();
                output.SetLength(output.Position);

                return output.Position == header.DecodedAllSize;
            }
        }

        private static bool Validate(Stream input)
        {
            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                var header = new FileHeader();
                var chapter = new Th06.Chapter();

                header.ReadFrom(reader);
                var remainSize = header.DecodedAllSize - header.Size;
                if (remainSize <= 0)
                    return false;

                try
                {
                    while (remainSize > 0)
                    {
                        chapter.ReadFrom(reader);
                        if (chapter.Size1 == 0)
                            return false;

                        switch (chapter.Signature)
                        {
                            case Header.ValidSignature:
                                if (chapter.FirstByteOfData != 0x01)
                                    return false;
                                break;
                            default:
                                break;
                        }

                        remainSize -= chapter.Size1;
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
            var dictionary = new Dictionary<string, Action<AllScoreData, Th06.Chapter>>
            {
                { Header.ValidSignature,           (data, ch) => data.Set(new Header(ch))           },
                { HighScore.ValidSignature,        (data, ch) => data.Set(new HighScore(ch))        },
                { PlayStatus.ValidSignature,       (data, ch) => data.Set(new PlayStatus(ch))       },
                { Th07.LastName.ValidSignature,    (data, ch) => data.Set(new Th07.LastName(ch))    },
                { Th07.VersionInfo.ValidSignature, (data, ch) => data.Set(new Th07.VersionInfo(ch)) },
            };

            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                var allScoreData = new AllScoreData();
                var chapter = new Th06.Chapter();

                reader.ReadExactBytes(FileHeader.ValidSize);

                try
                {
                    while (true)
                    {
                        chapter.ReadFrom(reader);
                        if (dictionary.TryGetValue(chapter.Signature, out Action<AllScoreData, Th06.Chapter> setChapter))
                            setChapter(allScoreData, chapter);
                    }
                }
                catch (EndOfStreamException)
                {
                    // It's OK, do nothing.
                }

                var numCharas = Enum.GetValues(typeof(Chara)).Length;
                var numLevels = Enum.GetValues(typeof(Level)).Length;
                if ((allScoreData.Header != null) &&
                    (allScoreData.Rankings.Count == numCharas * numLevels) &&
                    (allScoreData.PlayStatus != null) &&
                    (allScoreData.LastName != null) &&
                    (allScoreData.VersionInfo != null))
                    return allScoreData;
                else
                    return null;
            }
        }

        // %T09SCR[w][xx][y][z]
        private class ScoreReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T09SCR({0})({1})([1-5])([1-3])", LevelParser.Pattern, CharaParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ScoreReplacer(Th09Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = CharaParser.Parse(match.Groups[2].Value);
                    var rank = Utils.ToZeroBased(
                        int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                    var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    var score = parent.allScoreData.Rankings[(chara, level)][rank];
                    var date = string.Empty;

                    switch (type)
                    {
                        case 1:     // name
                            return Encoding.Default.GetString(score.Name).Split('\0')[0];
                        case 2:     // score
                            return Utils.ToNumberString((score.Score * 10) + score.ContinueCount);
                        case 3:     // date
                            date = Encoding.Default.GetString(score.Date).Split('\0')[0];
                            return (date != "--/--") ? date : "--/--/--";
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

        // %T09TIMEALL
        private class TimeReplacer : IStringReplaceable
        {
            private const string Pattern = @"%T09TIMEALL";

            private readonly MatchEvaluator evaluator;

            public TimeReplacer(Th09Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    return parent.allScoreData.PlayStatus.TotalRunningTime.ToLongString();
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T09CLEAR[x][yy][z]
        private class ClearReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T09CLEAR({0})({1})([12])", LevelParser.Pattern, CharaParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ClearReplacer(Th09Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = CharaParser.Parse(match.Groups[2].Value);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    var count = parent.allScoreData.PlayStatus.ClearCounts[chara].Counts[level];

                    if (type == 1)
                    {
                        return Utils.ToNumberString(count);
                    }
                    else
                    {
                        if (count > 0)
                        {
                            return "Cleared";
                        }
                        else
                        {
                            var score = parent.allScoreData.Rankings[(chara, level)][0];
                            var date = Encoding.Default.GetString(score.Date).TrimEnd('\0');
                            return (date != "--/--") ? "Not Cleared" : "-------";
                        }
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        private class AllScoreData
        {
            public AllScoreData()
            {
                var numPairs = Enum.GetValues(typeof(Chara)).Length * Enum.GetValues(typeof(Level)).Length;
                this.Rankings = new Dictionary<(Chara, Level), HighScore[]>(numPairs);
            }

            public Header Header { get; private set; }

            public Dictionary<(Chara, Level), HighScore[]> Rankings { get; private set; }

            public PlayStatus PlayStatus { get; private set; }

            public Th07.LastName LastName { get; private set; }

            public Th07.VersionInfo VersionInfo { get; private set; }

            public void Set(Header header) => this.Header = header;

            public void Set(HighScore score)
            {
                var key = (score.Chara, score.Level);
                if (!this.Rankings.ContainsKey(key))
                    this.Rankings.Add(key, new HighScore[5]);
                if ((score.Rank >= 0) && (score.Rank < 5))
                    this.Rankings[key][score.Rank] = score;
            }

            public void Set(PlayStatus status) => this.PlayStatus = status;

            public void Set(Th07.LastName name) => this.LastName = name;

            public void Set(Th07.VersionInfo info) => this.VersionInfo = info;
        }

        private class HighScore : Th06.Chapter   // per character, level, rank
        {
            public const string ValidSignature = "HSCR";
            public const short ValidSize = 0x002C;

            public HighScore(Th06.Chapter chapter)
                : base(chapter, ValidSignature, ValidSize)
            {
                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    reader.ReadUInt32();    // always 0x00000002?
                    this.Score = reader.ReadUInt32();
                    reader.ReadUInt32();    // always 0x00000000?
                    this.Chara = Utils.ToEnum<Chara>(reader.ReadByte());
                    this.Level = Utils.ToEnum<Level>(reader.ReadByte());
                    this.Rank = reader.ReadInt16();
                    this.Name = reader.ReadExactBytes(9);
                    this.Date = reader.ReadExactBytes(9);
                    reader.ReadByte();      // always 0x00?
                    this.ContinueCount = reader.ReadByte();
                }
            }

            public uint Score { get; }  // Divided by 10

            public Chara Chara { get; }

            public Level Level { get; }

            public short Rank { get; }  // 0-based

            public byte[] Name { get; } // Null-terminated

            public byte[] Date { get; } // "yy/mm/dd\0"

            public byte ContinueCount { get; }
        }

        private class PlayStatus : Th06.Chapter
        {
            public const string ValidSignature = "PLST";
            public const short ValidSize = 0x01FC;

            public PlayStatus(Th06.Chapter chapter)
                : base(chapter, ValidSignature, ValidSize)
            {
                var charas = Utils.GetEnumerator<Chara>();
                var numCharas = charas.Count();
                this.MatchFlags = new Dictionary<Chara, byte>(numCharas);
                this.StoryFlags = new Dictionary<Chara, byte>(numCharas);
                this.ExtraFlags = new Dictionary<Chara, byte>(numCharas);
                this.ClearCounts = new Dictionary<Chara, ClearCount>(numCharas);

                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    reader.ReadUInt32();    // always 0x00000003?
                    var hours = reader.ReadInt32();
                    var minutes = reader.ReadInt32();
                    var seconds = reader.ReadInt32();
                    var milliseconds = reader.ReadInt32();
                    this.TotalRunningTime = new Time(hours, minutes, seconds, milliseconds, false);
                    hours = reader.ReadInt32();
                    minutes = reader.ReadInt32();
                    seconds = reader.ReadInt32();
                    milliseconds = reader.ReadInt32();
                    this.TotalPlayTime = new Time(hours, minutes, seconds, milliseconds, false);
                    this.BgmFlags = reader.ReadExactBytes(19);
                    reader.ReadExactBytes(13);
                    foreach (var chara in charas)
                        this.MatchFlags.Add(chara, reader.ReadByte());
                    foreach (var chara in charas)
                        this.StoryFlags.Add(chara, reader.ReadByte());
                    foreach (var chara in charas)
                        this.ExtraFlags.Add(chara, reader.ReadByte());
                    foreach (var chara in charas)
                    {
                        var clearCount = new ClearCount();
                        clearCount.ReadFrom(reader);
                        this.ClearCounts.Add(chara, clearCount);
                    }
                }
            }

            public Time TotalRunningTime { get; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Time TotalPlayTime { get; }  // really...?

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public byte[] BgmFlags { get; }

            public Dictionary<Chara, byte> MatchFlags { get; }

            public Dictionary<Chara, byte> StoryFlags { get; }

            public Dictionary<Chara, byte> ExtraFlags { get; }

            public Dictionary<Chara, ClearCount> ClearCounts { get; }
        }

        private class ClearCount : IBinaryReadable
        {
            public ClearCount() => this.Counts = new Dictionary<Level, int>(Enum.GetValues(typeof(Level)).Length);

            public Dictionary<Level, int> Counts { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader == null)
                    throw new ArgumentNullException(nameof(reader));

                foreach (var level in Utils.GetEnumerator<Level>())
                    this.Counts.Add(level, reader.ReadInt32());
                reader.ReadUInt32();
            }
        }
    }
}
