﻿//-----------------------------------------------------------------------
// <copyright file="Definitions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System.Collections.Generic;
using System.Linq;
using CardInfo = ThScoreFileConverter.Models.SpellCardInfo<
    ThScoreFileConverter.Models.Stage, ThScoreFileConverter.Models.Level>;

namespace ThScoreFileConverter.Models.Th11
{
    internal static class Definitions
    {
        // Thanks to thwiki.info
        public static IReadOnlyDictionary<int, CardInfo> CardTable { get; } = new List<CardInfo>()
        {
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
            new CardInfo(  1, "怪奇「釣瓶落としの怪」",               Stage.One,   Level.Hard),
            new CardInfo(  2, "怪奇「釣瓶落としの怪」",               Stage.One,   Level.Lunatic),
            new CardInfo(  3, "罠符「キャプチャーウェブ」",           Stage.One,   Level.Easy),
            new CardInfo(  4, "罠符「キャプチャーウェブ」",           Stage.One,   Level.Normal),
            new CardInfo(  5, "蜘蛛「石窟の蜘蛛の巣」",               Stage.One,   Level.Hard),
            new CardInfo(  6, "蜘蛛「石窟の蜘蛛の巣」",               Stage.One,   Level.Lunatic),
            new CardInfo(  7, "瘴符「フィルドミアズマ」",             Stage.One,   Level.Easy),
            new CardInfo(  8, "瘴符「フィルドミアズマ」",             Stage.One,   Level.Normal),
            new CardInfo(  9, "瘴気「原因不明の熱病」",               Stage.One,   Level.Hard),
            new CardInfo( 10, "瘴気「原因不明の熱病」",               Stage.One,   Level.Lunatic),
            new CardInfo( 11, "妬符「グリーンアイドモンスター」",     Stage.Two,   Level.Easy),
            new CardInfo( 12, "妬符「グリーンアイドモンスター」",     Stage.Two,   Level.Normal),
            new CardInfo( 13, "嫉妬「緑色の目をした見えない怪物」",   Stage.Two,   Level.Hard),
            new CardInfo( 14, "嫉妬「緑色の目をした見えない怪物」",   Stage.Two,   Level.Lunatic),
            new CardInfo( 15, "花咲爺「華やかなる仁者への嫉妬」",     Stage.Two,   Level.Easy),
            new CardInfo( 16, "花咲爺「華やかなる仁者への嫉妬」",     Stage.Two,   Level.Normal),
            new CardInfo( 17, "花咲爺「シロの灰」",                   Stage.Two,   Level.Hard),
            new CardInfo( 18, "花咲爺「シロの灰」",                   Stage.Two,   Level.Lunatic),
            new CardInfo( 19, "舌切雀「謙虚なる富者への片恨」",       Stage.Two,   Level.Easy),
            new CardInfo( 20, "舌切雀「謙虚なる富者への片恨」",       Stage.Two,   Level.Normal),
            new CardInfo( 21, "舌切雀「大きな葛籠と小さな葛籠」",     Stage.Two,   Level.Hard),
            new CardInfo( 22, "舌切雀「大きな葛籠と小さな葛籠」",     Stage.Two,   Level.Lunatic),
            new CardInfo( 23, "恨符「丑の刻参り」",                   Stage.Two,   Level.Easy),
            new CardInfo( 24, "恨符「丑の刻参り」",                   Stage.Two,   Level.Normal),
            new CardInfo( 25, "恨符「丑の刻参り七日目」",             Stage.Two,   Level.Hard),
            new CardInfo( 26, "恨符「丑の刻参り七日目」",             Stage.Two,   Level.Lunatic),
            new CardInfo( 27, "鬼符「怪力乱神」",                     Stage.Three, Level.Easy),
            new CardInfo( 28, "鬼符「怪力乱神」",                     Stage.Three, Level.Normal),
            new CardInfo( 29, "鬼符「怪力乱神」",                     Stage.Three, Level.Hard),
            new CardInfo( 30, "鬼符「怪力乱神」",                     Stage.Three, Level.Lunatic),
            new CardInfo( 31, "怪輪「地獄の苦輪」",                   Stage.Three, Level.Easy),
            new CardInfo( 32, "怪輪「地獄の苦輪」",                   Stage.Three, Level.Normal),
            new CardInfo( 33, "枷符「咎人の外さぬ枷」",               Stage.Three, Level.Hard),
            new CardInfo( 34, "枷符「咎人の外さぬ枷」",               Stage.Three, Level.Lunatic),
            new CardInfo( 35, "力業「大江山嵐」",                     Stage.Three, Level.Easy),
            new CardInfo( 36, "力業「大江山嵐」",                     Stage.Three, Level.Normal),
            new CardInfo( 37, "力業「大江山颪」",                     Stage.Three, Level.Hard),
            new CardInfo( 38, "力業「大江山颪」",                     Stage.Three, Level.Lunatic),
            new CardInfo( 39, "四天王奥義「三歩必殺」",               Stage.Three, Level.Easy),
            new CardInfo( 40, "四天王奥義「三歩必殺」",               Stage.Three, Level.Normal),
            new CardInfo( 41, "四天王奥義「三歩必殺」",               Stage.Three, Level.Hard),
            new CardInfo( 42, "四天王奥義「三歩必殺」",               Stage.Three, Level.Lunatic),
            new CardInfo( 43, "想起「テリブルスーヴニール」",         Stage.Four,  Level.Easy),
            new CardInfo( 44, "想起「テリブルスーヴニール」",         Stage.Four,  Level.Normal),
            new CardInfo( 45, "想起「恐怖催眠術」",                   Stage.Four,  Level.Hard),
            new CardInfo( 46, "想起「恐怖催眠術」",                   Stage.Four,  Level.Lunatic),
            new CardInfo( 47, "想起「二重黒死蝶」",                   Stage.Four,  Level.Easy),
            new CardInfo( 48, "想起「二重黒死蝶」",                   Stage.Four,  Level.Normal),
            new CardInfo( 49, "想起「二重黒死蝶」",                   Stage.Four,  Level.Hard),
            new CardInfo( 50, "想起「二重黒死蝶」",                   Stage.Four,  Level.Lunatic),
            new CardInfo( 51, "想起「飛行虫ネスト」",                 Stage.Four,  Level.Easy),
            new CardInfo( 52, "想起「飛行虫ネスト」",                 Stage.Four,  Level.Normal),
            new CardInfo( 53, "想起「飛行虫ネスト」",                 Stage.Four,  Level.Hard),
            new CardInfo( 54, "想起「飛行虫ネスト」",                 Stage.Four,  Level.Lunatic),
            new CardInfo( 55, "想起「波と粒の境界」",                 Stage.Four,  Level.Easy),
            new CardInfo( 56, "想起「波と粒の境界」",                 Stage.Four,  Level.Normal),
            new CardInfo( 57, "想起「波と粒の境界」",                 Stage.Four,  Level.Hard),
            new CardInfo( 58, "想起「波と粒の境界」",                 Stage.Four,  Level.Lunatic),
            new CardInfo( 59, "想起「戸隠山投げ」",                   Stage.Four,  Level.Easy),
            new CardInfo( 60, "想起「戸隠山投げ」",                   Stage.Four,  Level.Normal),
            new CardInfo( 61, "想起「戸隠山投げ」",                   Stage.Four,  Level.Hard),
            new CardInfo( 62, "想起「戸隠山投げ」",                   Stage.Four,  Level.Lunatic),
            new CardInfo( 63, "想起「百万鬼夜行」",                   Stage.Four,  Level.Easy),
            new CardInfo( 64, "想起「百万鬼夜行」",                   Stage.Four,  Level.Normal),
            new CardInfo( 65, "想起「百万鬼夜行」",                   Stage.Four,  Level.Hard),
            new CardInfo( 66, "想起「百万鬼夜行」",                   Stage.Four,  Level.Lunatic),
            new CardInfo( 67, "想起「濛々迷霧」",                     Stage.Four,  Level.Easy),
            new CardInfo( 68, "想起「濛々迷霧」",                     Stage.Four,  Level.Normal),
            new CardInfo( 69, "想起「濛々迷霧」",                     Stage.Four,  Level.Hard),
            new CardInfo( 70, "想起「濛々迷霧」",                     Stage.Four,  Level.Lunatic),
            new CardInfo( 71, "想起「風神木の葉隠れ」",               Stage.Four,  Level.Easy),
            new CardInfo( 72, "想起「風神木の葉隠れ」",               Stage.Four,  Level.Normal),
            new CardInfo( 73, "想起「風神木の葉隠れ」",               Stage.Four,  Level.Hard),
            new CardInfo( 74, "想起「風神木の葉隠れ」",               Stage.Four,  Level.Lunatic),
            new CardInfo( 75, "想起「天狗のマクロバースト」",         Stage.Four,  Level.Easy),
            new CardInfo( 76, "想起「天狗のマクロバースト」",         Stage.Four,  Level.Normal),
            new CardInfo( 77, "想起「天狗のマクロバースト」",         Stage.Four,  Level.Hard),
            new CardInfo( 78, "想起「天狗のマクロバースト」",         Stage.Four,  Level.Lunatic),
            new CardInfo( 79, "想起「鳥居つむじ風」",                 Stage.Four,  Level.Easy),
            new CardInfo( 80, "想起「鳥居つむじ風」",                 Stage.Four,  Level.Normal),
            new CardInfo( 81, "想起「鳥居つむじ風」",                 Stage.Four,  Level.Hard),
            new CardInfo( 82, "想起「鳥居つむじ風」",                 Stage.Four,  Level.Lunatic),
            new CardInfo( 83, "想起「春の京人形」",                   Stage.Four,  Level.Easy),
            new CardInfo( 84, "想起「春の京人形」",                   Stage.Four,  Level.Normal),
            new CardInfo( 85, "想起「春の京人形」",                   Stage.Four,  Level.Hard),
            new CardInfo( 86, "想起「春の京人形」",                   Stage.Four,  Level.Lunatic),
            new CardInfo( 87, "想起「ストロードールカミカゼ」",       Stage.Four,  Level.Easy),
            new CardInfo( 88, "想起「ストロードールカミカゼ」",       Stage.Four,  Level.Normal),
            new CardInfo( 89, "想起「ストロードールカミカゼ」",       Stage.Four,  Level.Hard),
            new CardInfo( 90, "想起「ストロードールカミカゼ」",       Stage.Four,  Level.Lunatic),
            new CardInfo( 91, "想起「リターンイナニメトネス」",       Stage.Four,  Level.Easy),
            new CardInfo( 92, "想起「リターンイナニメトネス」",       Stage.Four,  Level.Normal),
            new CardInfo( 93, "想起「リターンイナニメトネス」",       Stage.Four,  Level.Hard),
            new CardInfo( 94, "想起「リターンイナニメトネス」",       Stage.Four,  Level.Lunatic),
            new CardInfo( 95, "想起「マーキュリポイズン」",           Stage.Four,  Level.Easy),
            new CardInfo( 96, "想起「マーキュリポイズン」",           Stage.Four,  Level.Normal),
            new CardInfo( 97, "想起「マーキュリポイズン」",           Stage.Four,  Level.Hard),
            new CardInfo( 98, "想起「マーキュリポイズン」",           Stage.Four,  Level.Lunatic),
            new CardInfo( 99, "想起「プリンセスウンディネ」",         Stage.Four,  Level.Easy),
            new CardInfo(100, "想起「プリンセスウンディネ」",         Stage.Four,  Level.Normal),
            new CardInfo(101, "想起「プリンセスウンディネ」",         Stage.Four,  Level.Hard),
            new CardInfo(102, "想起「プリンセスウンディネ」",         Stage.Four,  Level.Lunatic),
            new CardInfo(103, "想起「賢者の石」",                     Stage.Four,  Level.Easy),
            new CardInfo(104, "想起「賢者の石」",                     Stage.Four,  Level.Normal),
            new CardInfo(105, "想起「賢者の石」",                     Stage.Four,  Level.Hard),
            new CardInfo(106, "想起「賢者の石」",                     Stage.Four,  Level.Lunatic),
            new CardInfo(107, "想起「のびーるアーム」",               Stage.Four,  Level.Easy),
            new CardInfo(108, "想起「のびーるアーム」",               Stage.Four,  Level.Normal),
            new CardInfo(109, "想起「のびーるアーム」",               Stage.Four,  Level.Hard),
            new CardInfo(110, "想起「のびーるアーム」",               Stage.Four,  Level.Lunatic),
            new CardInfo(111, "想起「河童のポロロッカ」",             Stage.Four,  Level.Easy),
            new CardInfo(112, "想起「河童のポロロッカ」",             Stage.Four,  Level.Normal),
            new CardInfo(113, "想起「河童のポロロッカ」",             Stage.Four,  Level.Hard),
            new CardInfo(114, "想起「河童のポロロッカ」",             Stage.Four,  Level.Lunatic),
            new CardInfo(115, "想起「光り輝く水底のトラウマ」",       Stage.Four,  Level.Easy),
            new CardInfo(116, "想起「光り輝く水底のトラウマ」",       Stage.Four,  Level.Normal),
            new CardInfo(117, "想起「光り輝く水底のトラウマ」",       Stage.Four,  Level.Hard),
            new CardInfo(118, "想起「光り輝く水底のトラウマ」",       Stage.Four,  Level.Lunatic),
            new CardInfo(119, "猫符「キャッツウォーク」",             Stage.Five,  Level.Easy),
            new CardInfo(120, "猫符「キャッツウォーク」",             Stage.Five,  Level.Normal),
            new CardInfo(121, "猫符「怨霊猫乱歩」",                   Stage.Five,  Level.Hard),
            new CardInfo(122, "猫符「怨霊猫乱歩」",                   Stage.Five,  Level.Lunatic),
            new CardInfo(123, "呪精「ゾンビフェアリー」",             Stage.Five,  Level.Easy),
            new CardInfo(124, "呪精「ゾンビフェアリー」",             Stage.Five,  Level.Normal),
            new CardInfo(125, "呪精「怨霊憑依妖精」",                 Stage.Five,  Level.Hard),
            new CardInfo(126, "呪精「怨霊憑依妖精」",                 Stage.Five,  Level.Lunatic),
            new CardInfo(127, "恨霊「スプリーンイーター」",           Stage.Five,  Level.Easy),
            new CardInfo(128, "恨霊「スプリーンイーター」",           Stage.Five,  Level.Normal),
            new CardInfo(129, "屍霊「食人怨霊」",                     Stage.Five,  Level.Hard),
            new CardInfo(130, "屍霊「食人怨霊」",                     Stage.Five,  Level.Lunatic),
            new CardInfo(131, "贖罪「旧地獄の針山」",                 Stage.Five,  Level.Easy),
            new CardInfo(132, "贖罪「旧地獄の針山」",                 Stage.Five,  Level.Normal),
            new CardInfo(133, "贖罪「昔時の針と痛がる怨霊」",         Stage.Five,  Level.Hard),
            new CardInfo(134, "贖罪「昔時の針と痛がる怨霊」",         Stage.Five,  Level.Lunatic),
            new CardInfo(135, "「死灰復燃」",                         Stage.Five,  Level.Easy),
            new CardInfo(136, "「死灰復燃」",                         Stage.Five,  Level.Normal),
            new CardInfo(137, "「小悪霊復活せし」",                   Stage.Five,  Level.Hard),
            new CardInfo(138, "「小悪霊復活せし」",                   Stage.Five,  Level.Lunatic),
            new CardInfo(139, "妖怪「火焔の車輪」",                   Stage.Six,   Level.Easy),
            new CardInfo(140, "妖怪「火焔の車輪」",                   Stage.Six,   Level.Normal),
            new CardInfo(141, "妖怪「火焔の車輪」",                   Stage.Six,   Level.Hard),
            new CardInfo(142, "妖怪「火焔の車輪」",                   Stage.Six,   Level.Lunatic),
            new CardInfo(143, "核熱「ニュークリアフュージョン」",     Stage.Six,   Level.Easy),
            new CardInfo(144, "核熱「ニュークリアフュージョン」",     Stage.Six,   Level.Normal),
            new CardInfo(145, "核熱「ニュークリアエクスカーション」", Stage.Six,   Level.Hard),
            new CardInfo(146, "核熱「核反応制御不能」",               Stage.Six,   Level.Lunatic),
            new CardInfo(147, "爆符「プチフレア」",                   Stage.Six,   Level.Easy),
            new CardInfo(148, "爆符「メガフレア」",                   Stage.Six,   Level.Normal),
            new CardInfo(149, "爆符「ギガフレア」",                   Stage.Six,   Level.Hard),
            new CardInfo(150, "爆符「ペタフレア」",                   Stage.Six,   Level.Lunatic),
            new CardInfo(151, "焔星「フィクストスター」",             Stage.Six,   Level.Easy),
            new CardInfo(152, "焔星「フィクストスター」",             Stage.Six,   Level.Normal),
            new CardInfo(153, "焔星「プラネタリーレボリューション」", Stage.Six,   Level.Hard),
            new CardInfo(154, "焔星「十凶星」",                       Stage.Six,   Level.Lunatic),
            new CardInfo(155, "「地獄極楽メルトダウン」",             Stage.Six,   Level.Easy),
            new CardInfo(156, "「地獄極楽メルトダウン」",             Stage.Six,   Level.Normal),
            new CardInfo(157, "「ヘルズトカマク」",                   Stage.Six,   Level.Hard),
            new CardInfo(158, "「ヘルズトカマク」",                   Stage.Six,   Level.Lunatic),
            new CardInfo(159, "「地獄の人工太陽」",                   Stage.Six,   Level.Easy),
            new CardInfo(160, "「地獄の人工太陽」",                   Stage.Six,   Level.Normal),
            new CardInfo(161, "「サブタレイニアンサン」",             Stage.Six,   Level.Hard),
            new CardInfo(162, "「サブタレイニアンサン」",             Stage.Six,   Level.Lunatic),
            new CardInfo(163, "秘法「九字刺し」",                     Stage.Extra, Level.Extra),
            new CardInfo(164, "奇跡「ミラクルフルーツ」",             Stage.Extra, Level.Extra),
            new CardInfo(165, "神徳「五穀豊穣ライスシャワー」",       Stage.Extra, Level.Extra),
            new CardInfo(166, "表象「夢枕にご先祖総立ち」",           Stage.Extra, Level.Extra),
            new CardInfo(167, "表象「弾幕パラノイア」",               Stage.Extra, Level.Extra),
            new CardInfo(168, "本能「イドの解放」",                   Stage.Extra, Level.Extra),
            new CardInfo(169, "抑制「スーパーエゴ」",                 Stage.Extra, Level.Extra),
            new CardInfo(170, "反応「妖怪ポリグラフ」",               Stage.Extra, Level.Extra),
            new CardInfo(171, "無意識「弾幕のロールシャッハ」",       Stage.Extra, Level.Extra),
            new CardInfo(172, "復燃「恋の埋火」",                     Stage.Extra, Level.Extra),
            new CardInfo(173, "深層「無意識の遺伝子」",               Stage.Extra, Level.Extra),
            new CardInfo(174, "「嫌われ者のフィロソフィ」",           Stage.Extra, Level.Extra),
            new CardInfo(175, "「サブタレイニアンローズ」",           Stage.Extra, Level.Extra),
#pragma warning restore SA1008 // Opening parenthesis should be spaced correctly
        }.ToDictionary(card => card.Id);
    }
}