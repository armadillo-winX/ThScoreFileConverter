﻿//-----------------------------------------------------------------------
// <copyright file="Th10Converter.cs" company="None">
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
using ThScoreFileConverter.Extensions;
using ThScoreFileConverter.Models.Th10;
using CardInfo = ThScoreFileConverter.Models.SpellCardInfo<
    ThScoreFileConverter.Models.Stage, ThScoreFileConverter.Models.Level>;
using IClearData = ThScoreFileConverter.Models.Th10.IClearData<
    ThScoreFileConverter.Models.Th10Converter.CharaWithTotal, ThScoreFileConverter.Models.Th10Converter.StageProgress>;

namespace ThScoreFileConverter.Models
{
    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Reviewed.")]
    internal class Th10Converter : ThConverter
    {
        // Thanks to thwiki.info
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
        private static readonly Dictionary<int, CardInfo> CardTable =
            new List<CardInfo>()
            {
                new CardInfo(  1, "葉符「狂いの落葉」",                       Stage.One,   Level.Hard),
                new CardInfo(  2, "葉符「狂いの落葉」",                       Stage.One,   Level.Lunatic),
                new CardInfo(  3, "秋符「オータムスカイ」",                   Stage.One,   Level.Easy),
                new CardInfo(  4, "秋符「オータムスカイ」",                   Stage.One,   Level.Normal),
                new CardInfo(  5, "秋符「秋の空と乙女の心」",                 Stage.One,   Level.Hard),
                new CardInfo(  6, "秋符「秋の空と乙女の心」",                 Stage.One,   Level.Lunatic),
                new CardInfo(  7, "豊符「オヲトシハーベスター」",             Stage.One,   Level.Easy),
                new CardInfo(  8, "豊符「オヲトシハーベスター」",             Stage.One,   Level.Normal),
                new CardInfo(  9, "豊作「穀物神の約束」",                     Stage.One,   Level.Hard),
                new CardInfo( 10, "豊作「穀物神の約束」",                     Stage.One,   Level.Lunatic),
                new CardInfo( 11, "厄符「バッドフォーチュン」",               Stage.Two,   Level.Easy),
                new CardInfo( 12, "厄符「バッドフォーチュン」",               Stage.Two,   Level.Normal),
                new CardInfo( 13, "厄符「厄神様のバイオリズム」",             Stage.Two,   Level.Hard),
                new CardInfo( 14, "厄符「厄神様のバイオリズム」",             Stage.Two,   Level.Lunatic),
                new CardInfo( 15, "疵符「ブロークンアミュレット」",           Stage.Two,   Level.Easy),
                new CardInfo( 16, "疵符「ブロークンアミュレット」",           Stage.Two,   Level.Normal),
                new CardInfo( 17, "疵痕「壊されたお守り」",                   Stage.Two,   Level.Hard),
                new CardInfo( 18, "疵痕「壊されたお守り」",                   Stage.Two,   Level.Lunatic),
                new CardInfo( 19, "悪霊「ミスフォーチュンズホイール」",       Stage.Two,   Level.Easy),
                new CardInfo( 20, "悪霊「ミスフォーチュンズホイール」",       Stage.Two,   Level.Normal),
                new CardInfo( 21, "悲運「大鐘婆の火」",                       Stage.Two,   Level.Hard),
                new CardInfo( 22, "悲運「大鐘婆の火」",                       Stage.Two,   Level.Lunatic),
                new CardInfo( 23, "創符「ペインフロー」",                     Stage.Two,   Level.Easy),
                new CardInfo( 24, "創符「ペインフロー」",                     Stage.Two,   Level.Normal),
                new CardInfo( 25, "創符「流刑人形」",                         Stage.Two,   Level.Hard),
                new CardInfo( 26, "創符「流刑人形」",                         Stage.Two,   Level.Lunatic),
                new CardInfo( 27, "光学「オプティカルカモフラージュ」",       Stage.Three, Level.Easy),
                new CardInfo( 28, "光学「オプティカルカモフラージュ」",       Stage.Three, Level.Normal),
                new CardInfo( 29, "光学「ハイドロカモフラージュ」",           Stage.Three, Level.Hard),
                new CardInfo( 30, "光学「ハイドロカモフラージュ」",           Stage.Three, Level.Lunatic),
                new CardInfo( 31, "洪水「ウーズフラッディング」",             Stage.Three, Level.Easy),
                new CardInfo( 32, "洪水「ウーズフラッディング」",             Stage.Three, Level.Normal),
                new CardInfo( 33, "洪水「デリューヴィアルメア」",             Stage.Three, Level.Hard),
                new CardInfo( 34, "漂溺「光り輝く水底のトラウマ」",           Stage.Three, Level.Lunatic),
                new CardInfo( 35, "水符「河童のポロロッカ」",                 Stage.Three, Level.Easy),
                new CardInfo( 36, "水符「河童のポロロッカ」",                 Stage.Three, Level.Normal),
                new CardInfo( 37, "水符「河童のフラッシュフラッド」",         Stage.Three, Level.Hard),
                new CardInfo( 38, "水符「河童の幻想大瀑布」",                 Stage.Three, Level.Lunatic),
                new CardInfo( 39, "河童「お化けキューカンバー」",             Stage.Three, Level.Easy),
                new CardInfo( 40, "河童「お化けキューカンバー」",             Stage.Three, Level.Normal),
                new CardInfo( 41, "河童「のびーるアーム」",                   Stage.Three, Level.Hard),
                new CardInfo( 42, "河童「スピン・ザ・セファリックプレート」", Stage.Three, Level.Lunatic),
                new CardInfo( 43, "岐符「天の八衢」",                         Stage.Four,  Level.Easy),
                new CardInfo( 44, "岐符「天の八衢」",                         Stage.Four,  Level.Normal),
                new CardInfo( 45, "岐符「サルタクロス」",                     Stage.Four,  Level.Hard),
                new CardInfo( 46, "岐符「サルタクロス」",                     Stage.Four,  Level.Lunatic),
                new CardInfo( 47, "風神「風神木の葉隠れ」",                   Stage.Four,  Level.Easy),
                new CardInfo( 48, "風神「風神木の葉隠れ」",                   Stage.Four,  Level.Normal),
                new CardInfo( 49, "風神「天狗颪」",                           Stage.Four,  Level.Hard),
                new CardInfo( 50, "風神「二百十日」",                         Stage.Four,  Level.Lunatic),
                new CardInfo( 51, "「幻想風靡」",                             Stage.Four,  Level.Normal),
                new CardInfo( 52, "「幻想風靡」",                             Stage.Four,  Level.Hard),
                new CardInfo( 53, "「無双風神」",                             Stage.Four,  Level.Lunatic),
                new CardInfo( 54, "塞符「山神渡御」",                         Stage.Four,  Level.Easy),
                new CardInfo( 55, "塞符「山神渡御」",                         Stage.Four,  Level.Normal),
                new CardInfo( 56, "塞符「天孫降臨」",                         Stage.Four,  Level.Hard),
                new CardInfo( 57, "塞符「天上天下の照國」",                   Stage.Four,  Level.Lunatic),
                new CardInfo( 58, "秘術「グレイソーマタージ」",               Stage.Five,  Level.Easy),
                new CardInfo( 59, "秘術「グレイソーマタージ」",               Stage.Five,  Level.Normal),
                new CardInfo( 60, "秘術「忘却の祭儀」",                       Stage.Five,  Level.Hard),
                new CardInfo( 61, "秘術「一子相伝の弾幕」",                   Stage.Five,  Level.Lunatic),
                new CardInfo( 62, "奇跡「白昼の客星」",                       Stage.Five,  Level.Easy),
                new CardInfo( 63, "奇跡「白昼の客星」",                       Stage.Five,  Level.Normal),
                new CardInfo( 64, "奇跡「客星の明るい夜」",                   Stage.Five,  Level.Hard),
                new CardInfo( 65, "奇跡「客星の明るすぎる夜」",               Stage.Five,  Level.Lunatic),
                new CardInfo( 66, "開海「海が割れる日」",                     Stage.Five,  Level.Easy),
                new CardInfo( 67, "開海「海が割れる日」",                     Stage.Five,  Level.Normal),
                new CardInfo( 68, "開海「モーゼの奇跡」",                     Stage.Five,  Level.Hard),
                new CardInfo( 69, "開海「モーゼの奇跡」",                     Stage.Five,  Level.Lunatic),
                new CardInfo( 70, "準備「神風を喚ぶ星の儀式」",               Stage.Five,  Level.Easy),
                new CardInfo( 71, "準備「神風を喚ぶ星の儀式」",               Stage.Five,  Level.Normal),
                new CardInfo( 72, "準備「サモンタケミナカタ」",               Stage.Five,  Level.Hard),
                new CardInfo( 73, "準備「サモンタケミナカタ」",               Stage.Five,  Level.Lunatic),
                new CardInfo( 74, "奇跡「神の風」",                           Stage.Five,  Level.Easy),
                new CardInfo( 75, "奇跡「神の風」",                           Stage.Five,  Level.Normal),
                new CardInfo( 76, "大奇跡「八坂の神風」",                     Stage.Five,  Level.Hard),
                new CardInfo( 77, "大奇跡「八坂の神風」",                     Stage.Five,  Level.Lunatic),
                new CardInfo( 78, "神祭「エクスパンデッド・オンバシラ」",     Stage.Six,   Level.Easy),
                new CardInfo( 79, "神祭「エクスパンデッド・オンバシラ」",     Stage.Six,   Level.Normal),
                new CardInfo( 80, "奇祭「目処梃子乱舞」",                     Stage.Six,   Level.Hard),
                new CardInfo( 81, "奇祭「目処梃子乱舞」",                     Stage.Six,   Level.Lunatic),
                new CardInfo( 82, "筒粥「神の粥」",                           Stage.Six,   Level.Easy),
                new CardInfo( 83, "筒粥「神の粥」",                           Stage.Six,   Level.Normal),
                new CardInfo( 84, "忘穀「アンリメンバードクロップ」",         Stage.Six,   Level.Hard),
                new CardInfo( 85, "神穀「ディバイニングクロップ」",           Stage.Six,   Level.Lunatic),
                new CardInfo( 86, "贄符「御射山御狩神事」",                   Stage.Six,   Level.Easy),
                new CardInfo( 87, "贄符「御射山御狩神事」",                   Stage.Six,   Level.Normal),
                new CardInfo( 88, "神秘「葛井の清水」",                       Stage.Six,   Level.Hard),
                new CardInfo( 89, "神秘「ヤマトトーラス」",                   Stage.Six,   Level.Lunatic),
                new CardInfo( 90, "天流「お天水の奇跡」",                     Stage.Six,   Level.Easy),
                new CardInfo( 91, "天流「お天水の奇跡」",                     Stage.Six,   Level.Normal),
                new CardInfo( 92, "天竜「雨の源泉」",                         Stage.Six,   Level.Hard),
                new CardInfo( 93, "天竜「雨の源泉」",                         Stage.Six,   Level.Lunatic),
                new CardInfo( 94, "「マウンテン・オブ・フェイス」",           Stage.Six,   Level.Easy),
                new CardInfo( 95, "「マウンテン・オブ・フェイス」",           Stage.Six,   Level.Normal),
                new CardInfo( 96, "「風神様の神徳」",                         Stage.Six,   Level.Hard),
                new CardInfo( 97, "「風神様の神徳」",                         Stage.Six,   Level.Lunatic),
                new CardInfo( 98, "神符「水眼の如き美しき源泉」",             Stage.Extra, Level.Extra),
                new CardInfo( 99, "神符「杉で結ぶ古き縁」",                   Stage.Extra, Level.Extra),
                new CardInfo(100, "神符「神が歩かれた御神渡り」",             Stage.Extra, Level.Extra),
                new CardInfo(101, "開宴「二拝二拍一拝」",                     Stage.Extra, Level.Extra),
                new CardInfo(102, "土着神「手長足長さま」",                   Stage.Extra, Level.Extra),
                new CardInfo(103, "神具「洩矢の鉄の輪」",                     Stage.Extra, Level.Extra),
                new CardInfo(104, "源符「厭い川の翡翠」",                     Stage.Extra, Level.Extra),
                new CardInfo(105, "蛙狩「蛙は口ゆえ蛇に呑まるる」",           Stage.Extra, Level.Extra),
                new CardInfo(106, "土着神「七つの石と七つの木」",             Stage.Extra, Level.Extra),
                new CardInfo(107, "土着神「ケロちゃん風雨に負けず」",         Stage.Extra, Level.Extra),
                new CardInfo(108, "土着神「宝永四年の赤蛙」",                 Stage.Extra, Level.Extra),
                new CardInfo(109, "「諏訪大戦　～ 土着神話 vs 中央神話」",    Stage.Extra, Level.Extra),
                new CardInfo(110, "祟符「ミシャグジさま」",                   Stage.Extra, Level.Extra),
            }.ToDictionary(card => card.Id);

        private static readonly EnumShortNameParser<Chara> CharaParser =
            new EnumShortNameParser<Chara>();

        private static readonly EnumShortNameParser<CharaWithTotal> CharaWithTotalParser =
            new EnumShortNameParser<CharaWithTotal>();

        private AllScoreData allScoreData = null;

        public enum Chara
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("RA")] ReimuA,
            [EnumAltName("RB")] ReimuB,
            [EnumAltName("RC")] ReimuC,
            [EnumAltName("MA")] MarisaA,
            [EnumAltName("MB")] MarisaB,
            [EnumAltName("MC")] MarisaC,
#pragma warning restore SA1134 // Attributes should not share line
        }

        public enum CharaWithTotal
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("RA")] ReimuA,
            [EnumAltName("RB")] ReimuB,
            [EnumAltName("RC")] ReimuC,
            [EnumAltName("MA")] MarisaA,
            [EnumAltName("MB")] MarisaB,
            [EnumAltName("MC")] MarisaC,
            [EnumAltName("TL")] Total,
#pragma warning restore SA1134 // Attributes should not share line
        }

        public enum StageProgress
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("-------")]     None,
            [EnumAltName("Stage 1")]     St1,
            [EnumAltName("Stage 2")]     St2,
            [EnumAltName("Stage 3")]     St3,
            [EnumAltName("Stage 4")]     St4,
            [EnumAltName("Stage 5")]     St5,
            [EnumAltName("Stage 6")]     St6,
            [EnumAltName("Extra Stage")] Extra,
            [EnumAltName("All Clear")]   Clear,
#pragma warning restore SA1134 // Attributes should not share line
        }

        public override string SupportedVersions
        {
            get { return "1.00a"; }
        }

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th10decoded.dat", FileMode.Create, FileAccess.ReadWrite))
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
                new CareerReplacer(this),
                new CardReplacer(this, hideUntriedCards),
                new CollectRateReplacer(this),
                new ClearReplacer(this),
                new CharaReplacer(this),
                new CharaExReplacer(this),
                new PracticeReplacer(this),
            };
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
                var chapter = new Th10.Chapter();

                try
                {
                    while (remainSize > 0)
                    {
                        chapter.ReadFrom(reader);
                        if (!chapter.IsValid)
                            return false;
                        if (!ClearData.CanInitialize(chapter) && !Status.CanInitialize(chapter))
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
            var dictionary = new Dictionary<string, Action<AllScoreData, Th10.Chapter>>
            {
                { ClearData.ValidSignature, (data, ch) => data.Set(new ClearData(ch)) },
                { Status.ValidSignature,    (data, ch) => data.Set(new Status(ch))    },
            };

            using (var reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                var allScoreData = new AllScoreData();
                var chapter = new Th10.Chapter();

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
                    (allScoreData.ClearData.Count == Enum.GetValues(typeof(CharaWithTotal)).Length) &&
                    (allScoreData.Status != null))
                    return allScoreData;
                else
                    return null;
            }
        }

        // %T10SCR[w][xx][y][z]
        private class ScoreReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10SCR({0})({1})(\d)([1-5])", LevelParser.Pattern, CharaParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ScoreReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = (CharaWithTotal)CharaParser.Parse(match.Groups[2].Value);
                    var rank = Utils.ToZeroBased(
                        int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
                    var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    var ranking = parent.allScoreData.ClearData[chara].Rankings[level][rank];
                    switch (type)
                    {
                        case 1:     // name
                            return Encoding.Default.GetString(ranking.Name.ToArray()).Split('\0')[0];
                        case 2:     // score
                            return Utils.ToNumberString((ranking.Score * 10) + ranking.ContinueCount);
                        case 3:     // stage
                            if (ranking.DateTime == 0)
                                return StageProgress.None.ToShortName();
                            if (ranking.StageProgress == StageProgress.Extra)
                                return "Not Clear";
                            return ranking.StageProgress.ToShortName();
                        case 4:     // date & time
                            if (ranking.DateTime == 0)
                                return "----/--/-- --:--:--";
                            return new DateTime(1970, 1, 1).AddSeconds(ranking.DateTime).ToLocalTime()
                                .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture);
                        case 5:     // slow
                            if (ranking.DateTime == 0)
                                return "-----%";
                            return Utils.Format("{0:F3}%", ranking.SlowRate);
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

        // %T10C[xxx][yy][z]
        private class CareerReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10C(\d{{3}})({0})([12])", CharaWithTotalParser.Pattern);

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public CareerReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var number = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var chara = CharaWithTotalParser.Parse(match.Groups[2].Value);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    Func<ISpellCard<Level>, int> getCount;
                    if (type == 1)
                        getCount = (card => card.ClearCount);
                    else
                        getCount = (card => card.TrialCount);

                    var cards = parent.allScoreData.ClearData[chara].Cards;
                    if (number == 0)
                    {
                        return Utils.ToNumberString(cards.Values.Sum(getCount));
                    }
                    else if (CardTable.ContainsKey(number))
                    {
                        if (cards.TryGetValue(number, out var card))
                            return Utils.ToNumberString(getCount(card));
                        else
                            return "0";
                    }
                    else
                    {
                        return match.ToString();
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10CARD[xxx][y]
        private class CardReplacer : IStringReplaceable
        {
            private const string Pattern = @"%T10CARD(\d{3})([NR])";

            private readonly MatchEvaluator evaluator;

            public CardReplacer(Th10Converter parent, bool hideUntriedCards)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var number = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var type = match.Groups[2].Value.ToUpperInvariant();

                    if (CardTable.ContainsKey(number))
                    {
                        if (type == "N")
                        {
                            if (hideUntriedCards)
                            {
                                var cards = parent.allScoreData.ClearData[CharaWithTotal.Total].Cards;
                                if (!cards.TryGetValue(number, out var card) || !card.HasTried)
                                    return "??????????";
                            }

                            return CardTable[number].Name;
                        }
                        else
                        {
                            return CardTable[number].Level.ToString();
                        }
                    }
                    else
                    {
                        return match.ToString();
                    }
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10CRG[w][xx][y][z]
        private class CollectRateReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10CRG({0})({1})({2})([12])",
                LevelWithTotalParser.Pattern,
                CharaWithTotalParser.Pattern,
                StageWithTotalParser.Pattern);

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public CollectRateReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelWithTotalParser.Parse(match.Groups[1].Value);
                    var chara = CharaWithTotalParser.Parse(match.Groups[2].Value);
                    var stage = StageWithTotalParser.Parse(match.Groups[3].Value);
                    var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                    if (stage == StageWithTotal.Extra)
                        return match.ToString();

                    Func<ISpellCard<Level>, bool> findByStage;
                    if (stage == StageWithTotal.Total)
                        findByStage = (card => true);
                    else
                        findByStage = (card => CardTable[card.Id].Stage == (Stage)stage);

                    Func<ISpellCard<Level>, bool> findByLevel = (card => true);
                    switch (level)
                    {
                        case LevelWithTotal.Total:
                            // Do nothing
                            break;
                        case LevelWithTotal.Extra:
                            findByStage = (card => CardTable[card.Id].Stage == Stage.Extra);
                            break;
                        default:
                            findByLevel = (card => card.Level == (Level)level);
                            break;
                    }

                    Func<ISpellCard<Level>, bool> findByType;
                    if (type == 1)
                        findByType = (card => card.ClearCount > 0);
                    else
                        findByType = (card => card.TrialCount > 0);

                    return parent.allScoreData.ClearData[chara].Cards.Values
                        .Count(Utils.MakeAndPredicate(findByLevel, findByStage, findByType))
                        .ToString(CultureInfo.CurrentCulture);
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10CLEAR[x][yy]
        private class ClearReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10CLEAR({0})({1})", LevelParser.Pattern, CharaParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ClearReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = (CharaWithTotal)CharaParser.Parse(match.Groups[2].Value);

                    var rankings = parent.allScoreData.ClearData[chara].Rankings[level]
                        .Where(ranking => ranking.DateTime > 0);
                    var stageProgress = rankings.Any()
                        ? rankings.Max(ranking => ranking.StageProgress) : StageProgress.None;

                    return (stageProgress == StageProgress.Extra)
                        ? "Not Clear" : stageProgress.ToShortName();
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10CHARA[xx][y]
        private class CharaReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10CHARA({0})([1-3])", CharaWithTotalParser.Pattern);

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public CharaReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var chara = CharaWithTotalParser.Parse(match.Groups[1].Value);
                    var type = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    Func<IClearData, long> getValueByType;
                    Func<long, string> toString;
                    if (type == 1)
                    {
                        getValueByType = (data => data.TotalPlayCount);
                        toString = Utils.ToNumberString;
                    }
                    else if (type == 2)
                    {
                        getValueByType = (data => data.PlayTime);
                        toString = (value => new Time(value).ToString());
                    }
                    else
                    {
                        getValueByType = (data => data.ClearCounts.Values.Sum());
                        toString = Utils.ToNumberString;
                    }

                    Func<AllScoreData, long> getValueByChara;
                    if (chara == CharaWithTotal.Total)
                    {
                        getValueByChara = (allData => allData.ClearData.Values
                            .Where(data => data.Chara != chara).Sum(getValueByType));
                    }
                    else
                    {
                        getValueByChara = (allData => getValueByType(allData.ClearData[chara]));
                    }

                    return toString(getValueByChara(parent.allScoreData));
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10CHARAEX[x][yy][z]
        private class CharaExReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10CHARAEX({0})({1})([1-3])",
                LevelWithTotalParser.Pattern,
                CharaWithTotalParser.Pattern);

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public CharaExReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelWithTotalParser.Parse(match.Groups[1].Value);
                    var chara = CharaWithTotalParser.Parse(match.Groups[2].Value);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    Func<IClearData, long> getValueByType;
                    Func<long, string> toString;
                    if (type == 1)
                    {
                        getValueByType = (data => data.TotalPlayCount);
                        toString = Utils.ToNumberString;
                    }
                    else if (type == 2)
                    {
                        getValueByType = (data => data.PlayTime);
                        toString = (value => new Time(value).ToString());
                    }
                    else
                    {
                        if (level == LevelWithTotal.Total)
                            getValueByType = (data => data.ClearCounts.Values.Sum());
                        else
                            getValueByType = (data => data.ClearCounts[(Level)level]);
                        toString = Utils.ToNumberString;
                    }

                    Func<AllScoreData, long> getValueByChara;
                    if (chara == CharaWithTotal.Total)
                    {
                        getValueByChara = (allData => allData.ClearData.Values
                            .Where(data => data.Chara != chara).Sum(getValueByType));
                    }
                    else
                    {
                        getValueByChara = (allData => getValueByType(allData.ClearData[chara]));
                    }

                    return toString(getValueByChara(parent.allScoreData));
                });
            }

            public string Replace(string input)
            {
                return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
            }
        }

        // %T10PRAC[x][yy][z]
        private class PracticeReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(
                @"%T10PRAC({0})({1})({2})", LevelParser.Pattern, CharaParser.Pattern, StageParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public PracticeReplacer(Th10Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var level = LevelParser.Parse(match.Groups[1].Value);
                    var chara = (CharaWithTotal)CharaParser.Parse(match.Groups[2].Value);
                    var stage = StageParser.Parse(match.Groups[3].Value);

                    if (level == Level.Extra)
                        return match.ToString();
                    if (stage == Stage.Extra)
                        return match.ToString();

                    if (parent.allScoreData.ClearData.ContainsKey(chara))
                    {
                        var key = (level, stage);
                        var practices = parent.allScoreData.ClearData[chara].Practices;
                        return practices.ContainsKey(key)
                            ? Utils.ToNumberString(practices[key].Score * 10) : "0";
                    }
                    else
                    {
                        return "0";
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
            private readonly Dictionary<CharaWithTotal, IClearData> clearData;

            public AllScoreData()
            {
                this.clearData =
                    new Dictionary<CharaWithTotal, IClearData>(Enum.GetValues(typeof(CharaWithTotal)).Length);
            }

            public Th095.HeaderBase Header { get; private set; }

            public IReadOnlyDictionary<CharaWithTotal, IClearData> ClearData => this.clearData;

            public IStatus Status { get; private set; }

            public void Set(Th095.HeaderBase header) => this.Header = header;

            public void Set(IClearData data)
            {
                if (!this.clearData.ContainsKey(data.Chara))
                    this.clearData.Add(data.Chara, data);
            }

            public void Set(IStatus status) => this.Status = status;
        }

        private class ClearData : Chapter, IClearData   // per character
        {
            public const string ValidSignature = "CR";
            public const ushort ValidVersion = 0x0000;
            public const int ValidSize = 0x0000437C;

            public ClearData(Chapter chapter)
                : base(chapter, ValidSignature, ValidVersion, ValidSize)
            {
                var levels = Utils.GetEnumerator<Level>();
                var levelsExceptExtra = levels.Where(lv => lv != Level.Extra);
                var stages = Utils.GetEnumerator<Stage>();
                var stagesExceptExtra = stages.Where(st => st != Stage.Extra);

                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    this.Chara = (CharaWithTotal)reader.ReadInt32();

                    this.Rankings = levels.ToDictionary(
                        level => level,
                        _ => Enumerable.Range(0, 10).Select(rank =>
                        {
                            var score = new ScoreData();
                            score.ReadFrom(reader);
                            return score;
                        }).ToList() as IReadOnlyList<IScoreData<StageProgress>>);

                    this.TotalPlayCount = reader.ReadInt32();
                    this.PlayTime = reader.ReadInt32();
                    this.ClearCounts = levels.ToDictionary(level => level, _ => reader.ReadInt32());

                    this.Practices = levelsExceptExtra
                        .SelectMany(level => stagesExceptExtra.Select(stage => (level, stage)))
                        .ToDictionary(pair => pair, _ =>
                        {
                            var practice = new Practice();
                            practice.ReadFrom(reader);
                            return practice as IPractice;
                        });

                    this.Cards = Enumerable.Range(0, CardTable.Count).Select(_ =>
                    {
                        var card = new SpellCard();
                        card.ReadFrom(reader);
                        return card as ISpellCard<Level>;
                    }).ToDictionary(card => card.Id);
                }
            }

            public CharaWithTotal Chara { get; }

            public IReadOnlyDictionary<Level, IReadOnlyList<IScoreData<StageProgress>>> Rankings { get; }

            public int TotalPlayCount { get; }

            public int PlayTime { get; }    // = seconds * 60fps

            public IReadOnlyDictionary<Level, int> ClearCounts { get; }

            public IReadOnlyDictionary<(Level, Stage), IPractice> Practices { get; }

            public IReadOnlyDictionary<int, ISpellCard<Level>> Cards { get; }

            public static bool CanInitialize(Chapter chapter)
            {
                return chapter.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                    && (chapter.Version == ValidVersion)
                    && (chapter.Size == ValidSize);
            }
        }

        private class Status : Chapter, IStatus
        {
            public const string ValidSignature = "ST";
            public const ushort ValidVersion = 0x0000;
            public const int ValidSize = 0x00000448;

            public Status(Chapter chapter)
                : base(chapter, ValidSignature, ValidVersion, ValidSize)
            {
                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    this.LastName = reader.ReadExactBytes(10);
                    reader.ReadExactBytes(0x10);
                    this.BgmFlags = reader.ReadExactBytes(18);
                    reader.ReadExactBytes(0x0410);
                }
            }

            public IEnumerable<byte> LastName { get; }  // The last 2 bytes are always 0x00 ?

            public IEnumerable<byte> BgmFlags { get; }

            public static bool CanInitialize(Chapter chapter)
            {
                return chapter.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                    && (chapter.Version == ValidVersion)
                    && (chapter.Size == ValidSize);
            }
        }

        private class ScoreData : ScoreDataBase<StageProgress>
        {
        }
    }
}
