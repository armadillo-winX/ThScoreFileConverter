﻿//-----------------------------------------------------------------------
// <copyright file="Th165Converter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable 1591
#pragma warning disable SA1600 // ElementsMustBeDocumented
#pragma warning disable SA1602 // EnumerationItemsMustBeDocumented

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed.")]

namespace ThScoreFileConverter.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
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

    internal class Th165Converter : ThConverter
    {
        // Thanks to thwiki.info
        private static readonly Dictionary<DayScenePair, EnemiesCardPair> SpellCards =
            new Dictionary<DayScenePair, EnemiesCardPair>()
            {
                { new DayScenePair(Day.Sunday,             1), new EnemiesCardPair(Enemy.Reimu,      string.Empty) },
                { new DayScenePair(Day.Sunday,             2), new EnemiesCardPair(Enemy.Reimu,      string.Empty) },
                { new DayScenePair(Day.Monday,             1), new EnemiesCardPair(Enemy.Seiran,     "弾符「イーグルシューティング」") },
                { new DayScenePair(Day.Monday,             2), new EnemiesCardPair(Enemy.Ringo,      "兎符「ストロベリー大ダンゴ」") },
                { new DayScenePair(Day.Monday,             3), new EnemiesCardPair(Enemy.Seiran,     "弾符「ラビットファルコナー」") },
                { new DayScenePair(Day.Monday,             4), new EnemiesCardPair(Enemy.Ringo,      "兎符「ダンゴ三姉妹」") },
                { new DayScenePair(Day.Tuesday,            1), new EnemiesCardPair(Enemy.Larva,      string.Empty) },
                { new DayScenePair(Day.Tuesday,            2), new EnemiesCardPair(Enemy.Larva,      "蝶符「バタフライドリーム」") },
                { new DayScenePair(Day.Tuesday,            3), new EnemiesCardPair(Enemy.Larva,      "蝶符「纏わり付く鱗粉」") },
                { new DayScenePair(Day.Wednesday,          1), new EnemiesCardPair(Enemy.Marisa,     string.Empty) },
                { new DayScenePair(Day.Wednesday,          2), new EnemiesCardPair(Enemy.Narumi,     "魔符「慈愛の地蔵」") },
                { new DayScenePair(Day.Wednesday,          3), new EnemiesCardPair(Enemy.Narumi,     "地蔵「菩薩ストンプ」") },
                { new DayScenePair(Day.Wednesday,          4), new EnemiesCardPair(Enemy.Narumi,     "地蔵「活きの良いバレットゴーレム」") },
                { new DayScenePair(Day.Thursday,           1), new EnemiesCardPair(Enemy.Nemuno,     string.Empty) },
                { new DayScenePair(Day.Thursday,           2), new EnemiesCardPair(Enemy.Nemuno,     "研符「狂い輝く鬼包丁」") },
                { new DayScenePair(Day.Thursday,           3), new EnemiesCardPair(Enemy.Nemuno,     "殺符「窮僻の山姥」") },
                { new DayScenePair(Day.Friday,             1), new EnemiesCardPair(Enemy.Aunn,       string.Empty) },
                { new DayScenePair(Day.Friday,             2), new EnemiesCardPair(Enemy.Aunn,       "独楽「コマ犬大回転」") },
                { new DayScenePair(Day.Friday,             3), new EnemiesCardPair(Enemy.Aunn,       "独楽「阿吽の閃光」") },
                { new DayScenePair(Day.Saturday,           1), new EnemiesCardPair(Enemy.Doremy,     string.Empty) },
                { new DayScenePair(Day.WrongSunday,        1), new EnemiesCardPair(Enemy.Reimu,      string.Empty) },
                { new DayScenePair(Day.WrongSunday,        2), new EnemiesCardPair(Enemy.Seiran,     "夢弾「ルナティックドリームショット」") },
                { new DayScenePair(Day.WrongSunday,        3), new EnemiesCardPair(Enemy.Ringo,      "団子「ダンゴフラワー」") },
                { new DayScenePair(Day.WrongSunday,        4), new EnemiesCardPair(Enemy.Larva,      "夢蝶「クレージーバタフライ」") },
                { new DayScenePair(Day.WrongSunday,        5), new EnemiesCardPair(Enemy.Narumi,     "夢地蔵「劫火の希望」") },
                { new DayScenePair(Day.WrongSunday,        6), new EnemiesCardPair(Enemy.Nemuno,     "夢尽「殺人鬼の懐」") },
                { new DayScenePair(Day.WrongSunday,        7), new EnemiesCardPair(Enemy.Aunn,       "夢犬「１０１匹の野良犬」") },
                { new DayScenePair(Day.WrongMonday,        1), new EnemiesCardPair(Enemy.Clownpiece, string.Empty) },
                { new DayScenePair(Day.WrongMonday,        2), new EnemiesCardPair(Enemy.Clownpiece, "獄符「バースティンググラッジ」") },
                { new DayScenePair(Day.WrongMonday,        3), new EnemiesCardPair(Enemy.Clownpiece, "獄符「ダブルストライプ」") },
                { new DayScenePair(Day.WrongMonday,        4), new EnemiesCardPair(Enemy.Clownpiece, "月夢「エクリプスナイトメア」") },
                { new DayScenePair(Day.WrongTuesday,       1), new EnemiesCardPair(Enemy.Sagume,     string.Empty) },
                { new DayScenePair(Day.WrongTuesday,       2), new EnemiesCardPair(Enemy.Sagume,     "玉符「金城鉄壁の陰陽玉」") },
                { new DayScenePair(Day.WrongTuesday,       3), new EnemiesCardPair(Enemy.Sagume,     "玉符「神々の写し難い弾冠」") },
                { new DayScenePair(Day.WrongTuesday,       4), new EnemiesCardPair(Enemy.Sagume,     "夢鷺「片翼の夢鷺」") },
                { new DayScenePair(Day.WrongWednesday,     1), new EnemiesCardPair(Enemy.Doremy,     string.Empty) },
                { new DayScenePair(Day.WrongWednesday,     2), new EnemiesCardPair(Enemy.Mai,        "竹符「バンブーラビリンス」") },
                { new DayScenePair(Day.WrongWednesday,     3), new EnemiesCardPair(Enemy.Satono,     "茗荷「メスメリズムダンス」") },
                { new DayScenePair(Day.WrongWednesday,     4), new EnemiesCardPair(Enemy.Mai,        "笹符「タナバタスタードリーム」") },
                { new DayScenePair(Day.WrongWednesday,     5), new EnemiesCardPair(Enemy.Satono,     "冥加「ビハインドナイトメア」") },
                { new DayScenePair(Day.WrongWednesday,     6), new EnemiesCardPair(Enemy.Mai, Enemy.Satono, string.Empty) },
                { new DayScenePair(Day.WrongThursday,      1), new EnemiesCardPair(Enemy.Hecatia,    "異界「ディストーテッドファイア」") },
                { new DayScenePair(Day.WrongThursday,      2), new EnemiesCardPair(Enemy.Hecatia,    "異界「恨みがましい地獄の雨」") },
                { new DayScenePair(Day.WrongThursday,      3), new EnemiesCardPair(Enemy.Hecatia,    "月「コズミックレディエーション」") },
                { new DayScenePair(Day.WrongThursday,      4), new EnemiesCardPair(Enemy.Hecatia,    "異界「逢魔ガ刻　夢」") },
                { new DayScenePair(Day.WrongThursday,      5), new EnemiesCardPair(Enemy.Hecatia,    "「月が堕ちてくる！」") },
                { new DayScenePair(Day.WrongFriday,        1), new EnemiesCardPair(Enemy.Junko,      string.Empty) },
                { new DayScenePair(Day.WrongFriday,        2), new EnemiesCardPair(Enemy.Junko,      "「震え凍える悪夢」") },
                { new DayScenePair(Day.WrongFriday,        3), new EnemiesCardPair(Enemy.Junko,      "「サイケデリックマンダラ」") },
                { new DayScenePair(Day.WrongFriday,        4), new EnemiesCardPair(Enemy.Junko,      "「極めて威厳のある純光」") },
                { new DayScenePair(Day.WrongFriday,        5), new EnemiesCardPair(Enemy.Junko,      "「確実に悪夢で殺す為の弾幕」") },
                { new DayScenePair(Day.WrongSaturday,      1), new EnemiesCardPair(Enemy.Okina,      "秘儀「マターラスッカ」") },
                { new DayScenePair(Day.WrongSaturday,      2), new EnemiesCardPair(Enemy.Okina,      "秘儀「背面の邪炎」") },
                { new DayScenePair(Day.WrongSaturday,      3), new EnemiesCardPair(Enemy.Okina,      "後符「絶対秘神の後光」") },
                { new DayScenePair(Day.WrongSaturday,      4), new EnemiesCardPair(Enemy.Okina,      "秘儀「秘神の暗曜弾幕」") },
                { new DayScenePair(Day.WrongSaturday,      5), new EnemiesCardPair(Enemy.Okina,      "秘儀「神秘の玉繭」") },
                { new DayScenePair(Day.WrongSaturday,      6), new EnemiesCardPair(Enemy.Okina,      string.Empty) },
                { new DayScenePair(Day.NightmareSunday,    1), new EnemiesCardPair(Enemy.Remilia,  Enemy.Flandre,      "紅魔符「ブラッディカタストロフ」") },
                { new DayScenePair(Day.NightmareSunday,    2), new EnemiesCardPair(Enemy.Byakuren, Enemy.Miko,         "星神符「十七条の超人」") },
                { new DayScenePair(Day.NightmareSunday,    3), new EnemiesCardPair(Enemy.Remilia,  Enemy.Byakuren,     "紅星符「超人ブラッディナイフ」") },
                { new DayScenePair(Day.NightmareSunday,    4), new EnemiesCardPair(Enemy.Flandre,  Enemy.Miko,         "紅神符「十七条のカタストロフ」") },
                { new DayScenePair(Day.NightmareSunday,    5), new EnemiesCardPair(Enemy.Remilia,  Enemy.Miko,         "神紅符「ブラッディ十七条のレーザー」") },
                { new DayScenePair(Day.NightmareSunday,    6), new EnemiesCardPair(Enemy.Flandre,  Enemy.Byakuren,     "紅星符「超人カタストロフ行脚」") },
                { new DayScenePair(Day.NightmareMonday,    1), new EnemiesCardPair(Enemy.Yuyuko,   Enemy.Eiki,         "妖花符「バタフライストーム閻魔笏」") },
                { new DayScenePair(Day.NightmareMonday,    2), new EnemiesCardPair(Enemy.Kanako,   Enemy.Suwako,       "風神符「ミシャバシラ」") },
                { new DayScenePair(Day.NightmareMonday,    3), new EnemiesCardPair(Enemy.Yuyuko,   Enemy.Kanako,       "風妖符「死蝶オンバシラ」") },
                { new DayScenePair(Day.NightmareMonday,    4), new EnemiesCardPair(Enemy.Eiki,     Enemy.Suwako,       "風花符「ミシャグジ様の是非」") },
                { new DayScenePair(Day.NightmareMonday,    5), new EnemiesCardPair(Enemy.Yuyuko,   Enemy.Suwako,       "妖風符「土着蝶ストーム」") },
                { new DayScenePair(Day.NightmareMonday,    6), new EnemiesCardPair(Enemy.Eiki,     Enemy.Kanako,       "風花符「オンバシラ裁判」") },
                { new DayScenePair(Day.NightmareTuesday,   1), new EnemiesCardPair(Enemy.Eirin,    Enemy.Kaguya,       "永夜符「蓬莱壺中の弾の枝」") },
                { new DayScenePair(Day.NightmareTuesday,   2), new EnemiesCardPair(Enemy.Tenshi,   Enemy.Shinmyoumaru, "緋針符「要石も大きくなあれ」") },
                { new DayScenePair(Day.NightmareTuesday,   3), new EnemiesCardPair(Enemy.Eirin,    Enemy.Tenshi,       "永緋符「墜落する壺中の有頂天」") },
                { new DayScenePair(Day.NightmareTuesday,   4), new EnemiesCardPair(Enemy.Kaguya,   Enemy.Shinmyoumaru, "輝夜符「蓬莱の大きな弾の枝」") },
                { new DayScenePair(Day.NightmareTuesday,   5), new EnemiesCardPair(Enemy.Eirin,    Enemy.Shinmyoumaru, "永輝符「大きくなる壺」") },
                { new DayScenePair(Day.NightmareTuesday,   6), new EnemiesCardPair(Enemy.Kaguya,   Enemy.Tenshi,       "緋夜符「蓬莱の弾の要石」") },
                { new DayScenePair(Day.NightmareWednesday, 1), new EnemiesCardPair(Enemy.Satori,   Enemy.Utsuho,       "地霊符「マインドステラスチール」") },
                { new DayScenePair(Day.NightmareWednesday, 2), new EnemiesCardPair(Enemy.Ran,      Enemy.Koishi,       "地妖符「イドの式神」") },
                { new DayScenePair(Day.NightmareWednesday, 3), new EnemiesCardPair(Enemy.Satori,   Enemy.Koishi,       "「パーフェクトマインドコントロール」") },
                { new DayScenePair(Day.NightmareWednesday, 4), new EnemiesCardPair(Enemy.Ran,      Enemy.Utsuho,       "地妖符「式神大星」") },
                { new DayScenePair(Day.NightmareWednesday, 5), new EnemiesCardPair(Enemy.Ran,      Enemy.Satori,       "地妖符「エゴの式神」") },
                { new DayScenePair(Day.NightmareWednesday, 6), new EnemiesCardPair(Enemy.Utsuho,   Enemy.Koishi,       "地霊符「マインドステラリリーフ」") },
                { new DayScenePair(Day.NightmareThursday,  1), new EnemiesCardPair(Enemy.Nue,      Enemy.Mamizou,      "神星符「正体不明の怪光人だかり」") },
                { new DayScenePair(Day.NightmareThursday,  2), new EnemiesCardPair(Enemy.Iku,      Enemy.Raiko,        "輝天符「迅雷のドンドコ太鼓」") },
                { new DayScenePair(Day.NightmareThursday,  3), new EnemiesCardPair(Enemy.Mamizou,  Enemy.Raiko,        "輝神符「謎のドンドコ人だかり」") },
                { new DayScenePair(Day.NightmareThursday,  4), new EnemiesCardPair(Enemy.Iku,      Enemy.Nue,          "緋星符「正体不明の落雷」") },
                { new DayScenePair(Day.NightmareThursday,  5), new EnemiesCardPair(Enemy.Iku,      Enemy.Mamizou,      "神緋符「雷雨の中のストーカー」") },
                { new DayScenePair(Day.NightmareThursday,  6), new EnemiesCardPair(Enemy.Nue,      Enemy.Raiko,        "輝星符「正体不明のドンドコ太鼓」") },
                { new DayScenePair(Day.NightmareFriday,    1), new EnemiesCardPair(Enemy.Suika,    Enemy.Mokou,        "萃夜符「身命霧散」") },
                { new DayScenePair(Day.NightmareFriday,    2), new EnemiesCardPair(Enemy.Junko,    Enemy.Hecatia,      "紺珠符「純粋と不純の弾幕」") },
                { new DayScenePair(Day.NightmareFriday,    3), new EnemiesCardPair(Enemy.Suika,    Enemy.Junko,        "萃珠符「純粋な五里霧中」") },
                { new DayScenePair(Day.NightmareFriday,    4), new EnemiesCardPair(Enemy.Mokou,    Enemy.Hecatia,      "永珠符「捨て身のリフレクション」") },
                { new DayScenePair(Day.NightmareFriday,    5), new EnemiesCardPair(Enemy.Suika,    Enemy.Hecatia,      "萃珠符「ミストレイ」") },
                { new DayScenePair(Day.NightmareFriday,    6), new EnemiesCardPair(Enemy.Mokou,    Enemy.Junko,        "永珠符「穢れ無き珠と穢れ多き霊」") },
                { new DayScenePair(Day.NightmareSaturday,  1), new EnemiesCardPair(Enemy.Yukari,   Enemy.Okina,        "「秘神結界」") },
                { new DayScenePair(Day.NightmareSaturday,  2), new EnemiesCardPair(Enemy.Reimu,    Enemy.Marisa,       "「盗撮者調伏マスタースパーク」") },
                { new DayScenePair(Day.NightmareSaturday,  3), new EnemiesCardPair(Enemy.Reimu,    Enemy.Okina,        "「背後からの盗撮者調伏」") },
                { new DayScenePair(Day.NightmareSaturday,  4), new EnemiesCardPair(Enemy.Marisa,   Enemy.Yukari,       "「弾幕結界を撃ち抜け！」") },
                { new DayScenePair(Day.NightmareSaturday,  5), new EnemiesCardPair(Enemy.Marisa,   Enemy.Okina,        "「卑怯者マスタースパーク」") },
                { new DayScenePair(Day.NightmareSaturday,  6), new EnemiesCardPair(Enemy.Reimu,    Enemy.Yukari,       "「許可無く弾幕は撮影禁止です」") },
                { new DayScenePair(Day.NightmareDiary,     1), new EnemiesCardPair(Enemy.Doremy,                       "「最後の日曜日に見る悪夢」") },
                { new DayScenePair(Day.NightmareDiary,     2), new EnemiesCardPair(Enemy.Sumireko,                     "紙符「ＥＳＰカード手裏剣」") },
                { new DayScenePair(Day.NightmareDiary,     3), new EnemiesCardPair(Enemy.Sumireko, Enemy.Yukari,       "紙符「結界中のＥＳＰカード手裏剣」") },
                { new DayScenePair(Day.NightmareDiary,     4), new EnemiesCardPair(Enemy.DreamSumireko,                string.Empty) },
            };

        private static readonly List<string> Nicknames =
            new List<string>
            {
                "秘封倶楽部　伝説の会長",
                "現実を取り戻した会長",
                "弱小同好会",
                "注目の新規同好会",
                "噂のオカルト同好会",
                "一目置かれるオカルト部",
                "格上のオカルト部",
                "名の知れたオカルト部",
                "至極崇高なオカルト部",
                "信仰を集めるオカルト部",
                "神秘的なオカルト部",
                "ムーが食い付くオカルト部",
                "初めての弾幕写真",
                "終わらない悪夢",
                "真の悪夢の始まり",
                "夢の世界の終わり",
                "覚醒超能力者　菫子",
                "夢の支配者",
                "悪夢の支配者",
                "正夢の支配者",
                "ＳＮＳ始めました",
                "映え写真ガール",
                "枚数だけカメラマン",
                "瞬撮投稿ガール",
                "秘封イ○スタグラマー",
                "駆け出しバレスタグラマー",
                "人気バレスタグラマー",
                "超絶バレスタグラマー",
                "カリスマバレスタグラマー",
                "秘封グラマー",
                "会心の一枚",
                "奇跡の一枚",
                "究極の一枚",
                "神懸かった写真",
                "秘封を曝く写真",
                "うたた寝女子高生",
                "レム睡眠女子高生",
                "ショートスリーパー",
                "睡眠不足女子高生",
                "夢遊病女子高生",
                "ボロボロ会長",
                "ゾンビ会長",
                "被弾大好き会長",
                "ヒュンヒュン人間",
                "秘封テレポーター",
                "なんちゃって不死身ちゃん",
                "夢オチ不死身ちゃん",
                "スーパードリーマー",
                "パーフェクトドリーマー",
                "バイオレットドリーマー",
            };

        private static readonly EnumShortNameParser<Day> DayParser =
            new EnumShortNameParser<Day>();

        private static readonly string DayLongPattern =
            string.Join("|", Utils.GetEnumerator<Day>().Select(day => day.ToLongName()).ToArray());

        private AllScoreData allScoreData = null;

        private Dictionary<DayScenePair, BestShotPair> bestshots = null;

        public enum Day
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("01", LongName = "01")] Sunday,
            [EnumAltName("02", LongName = "02")] Monday,
            [EnumAltName("03", LongName = "03")] Tuesday,
            [EnumAltName("04", LongName = "04")] Wednesday,
            [EnumAltName("05", LongName = "05")] Thursday,
            [EnumAltName("06", LongName = "06")] Friday,
            [EnumAltName("07", LongName = "07")] Saturday,
            [EnumAltName("W1", LongName = "08")] WrongSunday,
            [EnumAltName("W2", LongName = "09")] WrongMonday,
            [EnumAltName("W3", LongName = "10")] WrongTuesday,
            [EnumAltName("W4", LongName = "11")] WrongWednesday,
            [EnumAltName("W5", LongName = "12")] WrongThursday,
            [EnumAltName("W6", LongName = "13")] WrongFriday,
            [EnumAltName("W7", LongName = "14")] WrongSaturday,
            [EnumAltName("N1", LongName = "15")] NightmareSunday,
            [EnumAltName("N2", LongName = "16")] NightmareMonday,
            [EnumAltName("N3", LongName = "17")] NightmareTuesday,
            [EnumAltName("N4", LongName = "18")] NightmareWednesday,
            [EnumAltName("N5", LongName = "19")] NightmareThursday,
            [EnumAltName("N6", LongName = "20")] NightmareFriday,
            [EnumAltName("N7", LongName = "21")] NightmareSaturday,
            [EnumAltName("ND", LongName = "22")] NightmareDiary,
#pragma warning restore SA1134 // Attributes should not share line
        }

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        public enum Enemy
        {
#pragma warning disable SA1134 // Attributes should not share line
            [EnumAltName("霊夢",           LongName = "博麗 霊夢")]                  Reimu,
            [EnumAltName("清蘭",           LongName = "清蘭")]                       Seiran,
            [EnumAltName("鈴瑚",           LongName = "鈴瑚")]                       Ringo,
            [EnumAltName("ラルバ",         LongName = "エタニティラルバ")]           Larva,
            [EnumAltName("魔理沙",         LongName = "霧雨 魔理沙")]                Marisa,
            [EnumAltName("成美",           LongName = "矢田寺 成美")]                Narumi,
            [EnumAltName("ネムノ",         LongName = "坂田 ネムノ")]                Nemuno,
            [EnumAltName("あうん",         LongName = "高麗野 あうん")]              Aunn,
            [EnumAltName("ドレミー",       LongName = "ドレミー・スイート")]         Doremy,
            [EnumAltName("クラウンピース", LongName = "クラウンピース")]             Clownpiece,
            [EnumAltName("サグメ",         LongName = "稀神 サグメ")]                Sagume,
            [EnumAltName("舞",             LongName = "丁礼田 舞")]                  Mai,
            [EnumAltName("里乃",           LongName = "爾子田 里乃")]                Satono,
            [EnumAltName("ヘカーティア",   LongName = "ヘカーティア・ラピスラズリ")] Hecatia,
            [EnumAltName("純狐",           LongName = "純狐")]                       Junko,
            [EnumAltName("隠岐奈",         LongName = "摩多羅 隠岐奈")]              Okina,
            [EnumAltName("レミリア",       LongName = "レミリア・スカーレット")]     Remilia,
            [EnumAltName("フランドール",   LongName = "フランドール・スカーレット")] Flandre,
            [EnumAltName("白蓮",           LongName = "聖 白蓮")]                    Byakuren,
            [EnumAltName("神子",           LongName = "豊聡耳 神子")]                Miko,
            [EnumAltName("幽々子",         LongName = "西行寺 幽々子")]              Yuyuko,
            [EnumAltName("映姫",           LongName = "四季映姫・ヤマザナドゥ")]     Eiki,
            [EnumAltName("神奈子",         LongName = "八坂 神奈子")]                Kanako,
            [EnumAltName("諏訪子",         LongName = "洩矢 諏訪子")]                Suwako,
            [EnumAltName("永琳",           LongName = "八意 永琳")]                  Eirin,
            [EnumAltName("輝夜",           LongName = "蓬莱山 輝夜")]                Kaguya,
            [EnumAltName("天子",           LongName = "比那名居 天子")]              Tenshi,
            [EnumAltName("針妙丸",         LongName = "少名 針妙丸")]                Shinmyoumaru,
            [EnumAltName("さとり",         LongName = "古明地 さとり")]              Satori,
            [EnumAltName("空",             LongName = "霊烏路 空")]                  Utsuho,
            [EnumAltName("藍",             LongName = "八雲 藍")]                    Ran,
            [EnumAltName("こいし",         LongName = "古明地 こいし")]              Koishi,
            [EnumAltName("ぬえ",           LongName = "封獣 ぬえ")]                  Nue,
            [EnumAltName("マミゾウ",       LongName = "二ッ岩 マミゾウ")]            Mamizou,
            [EnumAltName("衣玖",           LongName = "永江 衣玖")]                  Iku,
            [EnumAltName("雷鼓",           LongName = "堀川 雷鼓")]                  Raiko,
            [EnumAltName("萃香",           LongName = "伊吹 萃香")]                  Suika,
            [EnumAltName("妹紅",           LongName = "藤原 妹紅")]                  Mokou,
            [EnumAltName("紫",             LongName = "八雲 紫")]                    Yukari,
            [EnumAltName("菫子",           LongName = "宇佐見 菫子")]                Sumireko,
            [EnumAltName("菫子(夢)",       LongName = "宇佐見 菫子 (夢)")]           DreamSumireko,
#pragma warning restore SA1134 // Attributes should not share line
        }

        public override string SupportedVersions => "1.00a";

        public override bool HasBestShotConverter => true;

        protected override bool ReadScoreFile(Stream input)
        {
            using (var decrypted = new MemoryStream())
#if DEBUG
            using (var decoded = new FileStream("th165decoded.dat", FileMode.Create, FileAccess.ReadWrite))
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
                new ScoreTotalReplacer(this),
                new CardReplacer(this, hideUntriedCards),
                new NicknameReplacer(this),
                new TimeReplacer(this),
                new ShotReplacer(this, outputFilePath),
                new ShotExReplacer(this, outputFilePath),
            };
        }

        protected override string[] FilterBestShotFiles(string[] files)
        {
            var pattern = Utils.Format(@"bs({0})_\d{{2}}.dat", DayLongPattern);

            return files.Where(file => Regex.IsMatch(
                Path.GetFileName(file), pattern, RegexOptions.IgnoreCase)).ToArray();
        }

        protected override void ConvertBestShot(Stream input, Stream output)
        {
            using (var decoded = new MemoryStream())
            {
                var outputFile = output as FileStream;

                using (var reader = new BinaryReader(input, Encoding.Default, true))
                {
                    var header = new BestShotHeader();
                    header.ReadFrom(reader);

                    if (this.bestshots == null)
                        this.bestshots = new Dictionary<DayScenePair, BestShotPair>(SpellCards.Count);

                    var key = new DayScenePair(header.Weekday, header.Dream);
                    if (!this.bestshots.ContainsKey(key))
                        this.bestshots.Add(key, new BestShotPair(outputFile.Name, header));

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
            using (var reader = new BinaryReader(input, Encoding.Default, true))
            using (var writer = new BinaryWriter(output, Encoding.Default, true))
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
            using (var reader = new BinaryReader(input, Encoding.Default, true))
            using (var writer = new BinaryWriter(output, Encoding.Default, true))
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
            using (var reader = new BinaryReader(input, Encoding.Default, true))
            {
                var header = new Header();
                header.ReadFrom(reader);
                var remainSize = header.DecodedBodySize;
                var chapter = new Chapter();

                try
                {
                    while (remainSize > 0)
                    {
                        chapter.ReadFrom(reader);
                        if (!chapter.IsValid)
                            return false;
                        if (!Score.CanInitialize(chapter) &&
                            !Status.CanInitialize(chapter))
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

        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:CodeMustNotContainMultipleWhitespaceInARow", Justification = "Reviewed.")]
        private static AllScoreData Read(Stream input)
        {
            var dictionary = new Dictionary<string, Action<AllScoreData, Chapter>>
            {
                { Score.ValidSignature,  (data, ch) => data.Set(new Score(ch))  },
                { Status.ValidSignature, (data, ch) => data.Set(new Status(ch)) },
            };

            using (var reader = new BinaryReader(input, Encoding.Default, true))
            {
                var allScoreData = new AllScoreData();
                var chapter = new Chapter();

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

        // %T165SCR[xx][y][z]
        private class ScoreReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(@"%T165SCR({0})([1-7])([1-4])", DayParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ScoreReplacer(Th165Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var day = DayParser.Parse(match.Groups[1].Value);
                    var scene = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    var key = new DayScenePair(day, scene);
                    if (!SpellCards.ContainsKey(key))
                        return match.ToString();

                    var score = parent.allScoreData.Scores.Find(elem =>
                        (elem != null) && ((elem.Number >= 0) && (elem.Number < SpellCards.Count)) &&
                        SpellCards.ElementAt(elem.Number).Key.Equals(key));

                    switch (type)
                    {
                        case 1:     // high score
                            return (score != null) ? Utils.ToNumberString(score.HighScore) : "0";
                        case 2:     // challenge count
                            return (score != null) ? Utils.ToNumberString(score.ChallengeCount) : "0";
                        case 3:     // cleared count
                            return (score != null) ? Utils.ToNumberString(score.ClearCount) : "0";
                        case 4:     // num of photos
                            return (score != null) ? Utils.ToNumberString(score.NumPhotos) : "0";
                        default:    // unreachable
                            return match.ToString();
                    }
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165SCRTL[x]
        private class ScoreTotalReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(@"%T165SCRTL([1-6])");

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public ScoreTotalReplacer(Th165Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var type = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                    switch (type)
                    {
                        case 1:     // total score
                            return Utils.ToNumberString(parent.allScoreData.Scores.Sum(score => score.HighScore));
                        case 2:     // total of challenge counts
                            return Utils.ToNumberString(parent.allScoreData.Scores.Sum(score => score.ChallengeCount));
                        case 3:     // total of cleared counts
                            return Utils.ToNumberString(parent.allScoreData.Scores.Sum(score => score.ClearCount));
                        case 4:     // num of cleared scenes
                            return Utils.ToNumberString(
                                parent.allScoreData.Scores.Count(score => score.ClearCount > 0));
                        case 5:     // num of photos
                            return Utils.ToNumberString(parent.allScoreData.Scores.Sum(score => score.NumPhotos));
                        case 6:     // num of nicknames
                            return Utils.ToNumberString(
                                parent.allScoreData.Status.NicknameFlags.Count(flag => flag > 0));
                        default:    // unreachable
                            return match.ToString();
                    }
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165CARD[xx][y][z]
        private class CardReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(@"%T165CARD({0})([1-7])([12])", DayParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public CardReplacer(Th165Converter parent, bool hideUntriedCards)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var day = DayParser.Parse(match.Groups[1].Value);
                    var scene = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    var key = new DayScenePair(day, scene);
                    if (!SpellCards.ContainsKey(key))
                        return match.ToString();

                    if (hideUntriedCards)
                    {
                        var score = parent.allScoreData.Scores.Find(elem =>
                            (elem != null) && ((elem.Number >= 0) && (elem.Number < SpellCards.Count)) &&
                            SpellCards.ElementAt(elem.Number).Key.Equals(key));
                        if ((score == null) || (score.ChallengeCount <= 0))
                            return "??????????";
                    }

                    if (type == 1)
                    {
                        var enemies = SpellCards[key].Enemies;
                        if (enemies.Length == 1)
                        {
                            return SpellCards[key].Enemy.ToLongName();
                        }
                        else
                        {
                            return string.Join(" &amp; ", enemies.Select(enemy => enemy.ToLongName()).ToArray());
                        }
                    }
                    else
                    {
                        return SpellCards[key].Card;
                    }
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165NICK[xx]
        private class NicknameReplacer : IStringReplaceable
        {
            private const string Pattern = @"%T165NICK(\d{2})";

            private readonly MatchEvaluator evaluator;

            public NicknameReplacer(Th165Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var number = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                    if ((number > 0) && (number <= Nicknames.Count))
                    {
                        return (parent.allScoreData.Status.NicknameFlags[number] > 0)
                            ? Nicknames[number - 1] : "??????????";
                    }
                    else
                    {
                        return match.ToString();
                    }
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165TIMEPLY
        private class TimeReplacer : IStringReplaceable
        {
            private const string Pattern = @"%T165TIMEPLY";

            private readonly MatchEvaluator evaluator;

            public TimeReplacer(Th165Converter parent)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    return new Time(parent.allScoreData.Status.TotalPlayTime * 10, false).ToLongString();
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165SHOT[xx][y]
        private class ShotReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(@"%T165SHOT({0})([1-7])", DayParser.Pattern);

            private readonly MatchEvaluator evaluator;

            public ShotReplacer(Th165Converter parent, string outputFilePath)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var day = DayParser.Parse(match.Groups[1].Value);
                    var scene = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    var key = new DayScenePair(day, scene);
                    if (!SpellCards.ContainsKey(key))
                        return match.ToString();

                    if (!string.IsNullOrEmpty(outputFilePath) &&
                        parent.bestshots.TryGetValue(key, out var bestshot))
                    {
                        var relativePath = new Uri(outputFilePath)
                            .MakeRelativeUri(new Uri(bestshot.Path)).OriginalString;
                        var alternativeString = Utils.Format("SpellName: {0}", SpellCards[key].Card);
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

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        // %T165SHOTEX[xx][y][z]
        private class ShotExReplacer : IStringReplaceable
        {
            private static readonly string Pattern = Utils.Format(@"%T165SHOTEX({0})([1-7])([1-9])", DayParser.Pattern);

            private static readonly Func<BestShotHeader, List<Hashtag>> HashtagList =
                header => new List<Hashtag>
                {
                    new Hashtag(header.Fields.IsSelfie, "＃自撮り！"),
                    new Hashtag(header.Fields.IsTwoShot, "＃ツーショット！"),
                    new Hashtag(header.Fields.IsThreeShot, "＃スリーショット！"),
                    new Hashtag(header.Fields.TwoEnemiesTogether, "＃二人まとめて撮影した！"),
                    new Hashtag(header.Fields.EnemyIsPartlyInFrame, "＃敵が見切れてる"),
                    new Hashtag(header.Fields.WholeEnemyIsInFrame, "＃敵を収めたよ"),
                    new Hashtag(header.Fields.EnemyIsInMiddle, "＃敵がど真ん中"),
                    new Hashtag(header.Fields.PeaceSignAlongside, "＃並んでピース"),
                    new Hashtag(header.Fields.EnemiesAreTooClose, "＃二人が近すぎるｗ"),
                    new Hashtag(header.Fields.EnemiesAreOverlapping, "＃二人が重なってるｗｗ"),
                    new Hashtag(header.Fields.Closeup, "＃接写！"),
                    new Hashtag(header.Fields.QuiteCloseup, "＃かなりの接写！"),
                    new Hashtag(header.Fields.TooClose, "＃近すぎてぶつかるー！"),
                    new Hashtag(header.Fields.TooManyBullets, "＃弾多すぎｗ"),
                    new Hashtag(header.Fields.TooPlayfulBarrage, "＃弾幕ふざけすぎｗｗ"),
                    new Hashtag(header.Fields.TooDense, "＃ちょっ、密度濃すぎｗｗｗ"),
                    new Hashtag(header.Fields.BitDangerous, "＃ちょっと危なかった"),
                    new Hashtag(header.Fields.SeriouslyDangerous, "＃マジで危なかった"),
                    new Hashtag(header.Fields.ThoughtGonnaDie, "＃死ぬかと思った"),
                    new Hashtag(header.Fields.EnemyIsInFullView, "＃敵が丸見えｗ"),
                    new Hashtag(header.Fields.ManyReds, "＃赤色多いな"),
                    new Hashtag(header.Fields.ManyPurples, "＃紫色多いね"),
                    new Hashtag(header.Fields.ManyBlues, "＃青色多いよ"),
                    new Hashtag(header.Fields.ManyCyans, "＃水色多いし"),
                    new Hashtag(header.Fields.ManyGreens, "＃緑色多いねぇ"),
                    new Hashtag(header.Fields.ManyYellows, "＃黄色多いなぁ"),
                    new Hashtag(header.Fields.ManyOranges, "＃橙色多いお"),
                    new Hashtag(header.Fields.TooColorful, "＃カラフル過ぎｗ"),
                    new Hashtag(header.Fields.SevenColors, "＃七色全部揃った！"),
                    new Hashtag(header.Fields.Dazzling, "＃うおっ、まぶし！"),
                    new Hashtag(header.Fields.MoreDazzling, "＃ぐあ、眩しすぎるー！"),
                    new Hashtag(header.Fields.MostDazzling, "＃うあー、目が、目がー！"),
                    new Hashtag(header.Fields.EnemyIsUndamaged, "＃敵は無傷だ"),
                    new Hashtag(header.Fields.EnemyCanAfford, "＃敵はまだ余裕がある"),
                    new Hashtag(header.Fields.EnemyIsWeakened, "＃敵がだいぶ弱ってる"),
                    new Hashtag(header.Fields.EnemyIsDying, "＃敵が瀕死だ"),
                    new Hashtag(header.Fields.Finished, "＃トドメをさしたよ！"),
                    new Hashtag(header.Fields.FinishedTogether, "＃二人まとめてトドメ！"),
                    new Hashtag(header.Fields.Chased, "＃追い打ちしたよ！"),
                    new Hashtag(header.Fields.IsSuppository, "＃座薬ｗｗｗ"),
                    new Hashtag(header.Fields.IsButterflyLikeMoth, "＃蛾みたいな蝶だ！"),
                    new Hashtag(header.Fields.Scorching, "＃アチチ、焦げちゃうよ"),
                    new Hashtag(header.Fields.TooBigBullet, "＃弾、大きすぎでしょｗ"),
                    new Hashtag(header.Fields.ThrowingEdgedTools, "＃刃物投げんな (و｀ω´)6"),
                    new Hashtag(header.Fields.IsThunder, "＃ぎゃー、雷はスマホがー"),
                    new Hashtag(header.Fields.Snaky, "＃うねうねだー！"),
                    new Hashtag(header.Fields.LightLooksStopped, "＃光が止まって見える！"),
                    new Hashtag(header.Fields.IsSuperMoon, "＃スーパームーン！"),
                    new Hashtag(header.Fields.IsRockyBarrage, "＃岩の弾幕とかｗｗ"),
                    new Hashtag(header.Fields.IsStickDestroyingBarrage, "＃弾幕を破壊する棒……？"),
                    new Hashtag(header.Fields.IsLovelyHeart, "＃ラブリーハート！"),
                    new Hashtag(header.Fields.IsDrum, "＃ドンドコドンドコ"),
                    new Hashtag(header.Fields.Fluffy, "＃もふもふもふー"),
                    new Hashtag(header.Fields.IsDoggiePhoto, "＃わんわん写真"),
                    new Hashtag(header.Fields.IsAnimalPhoto, "＃アニマルフォト"),
                    new Hashtag(header.Fields.IsZoo, "＃動物園！"),
                    new Hashtag(header.Fields.IsMisty, "＃身体が霧状に！？"),
                    new Hashtag(header.Fields.WasScolded, "＃怒られちゃった……"),
                    new Hashtag(header.Fields.IsLandscapePhoto, "＃風景写真"),
                    new Hashtag(header.Fields.IsBoringPhoto, "＃何ともつまらない写真"),
                    new Hashtag(header.Fields.IsSumireko, "＃私こそが宇佐見菫子だ！"),
                };

            private readonly MatchEvaluator evaluator;

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1119:StatementMustNotUseUnnecessaryParenthesis", Justification = "Reviewed.")]
            public ShotExReplacer(Th165Converter parent, string outputFilePath)
            {
                this.evaluator = new MatchEvaluator(match =>
                {
                    var day = DayParser.Parse(match.Groups[1].Value);
                    var scene = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                    var type = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

                    var key = new DayScenePair(day, scene);
                    if (!SpellCards.ContainsKey(key))
                        return match.ToString();

                    if (!string.IsNullOrEmpty(outputFilePath) &&
                        parent.bestshots.TryGetValue(key, out var bestshot))
                    {
                        switch (type)
                        {
                            case 1:     // relative path to the bestshot file
                                return new Uri(outputFilePath).MakeRelativeUri(new Uri(bestshot.Path)).OriginalString;
                            case 2:     // width
                                return bestshot.Header.Width.ToString(CultureInfo.InvariantCulture);
                            case 3:     // height
                                return bestshot.Header.Height.ToString(CultureInfo.InvariantCulture);
                            case 4:     // date & time
                                return new DateTime(1970, 1, 1)
                                    .AddSeconds(bestshot.Header.DateTime).ToLocalTime()
                                    .ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture);
                            case 5:     // hashtags
                                var hashtags = HashtagList(bestshot.Header)
                                    .Where(hashtag => hashtag.Outputs)
                                    .Select(hashtag => hashtag.Name);
                                return string.Join(Environment.NewLine, hashtags.ToArray());
                            case 6:     // number of views
                                return Utils.ToNumberString(bestshot.Header.NumViewed);
                            case 7:     // number of likes
                                return Utils.ToNumberString(bestshot.Header.NumLikes);
                            case 8:     // number of favs
                                return Utils.ToNumberString(bestshot.Header.NumFavs);
                            case 9:     // score
                                return Utils.ToNumberString(bestshot.Header.Score);
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
                            case 4: return "----/--/-- --:--:--";
                            case 5: return string.Empty;
                            case 6: return "0";
                            case 7: return "0";
                            case 8: return "0";
                            case 9: return "0";
                            default: return match.ToString();
                        }
                    }
                });
            }

            public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }

        private class DayScenePair : Pair<Day, int>
        {
            public DayScenePair(Day day, int scene)
                : base(day, scene)
            {
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public Day Day => this.First;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public int Scene => this.Second;    // 1-based
        }

        private class EnemiesCardPair : Pair<Enemy[], string>
        {
            public EnemiesCardPair(Enemy enemy, string card)
                : base(new Enemy[] { enemy }, card)
            {
            }

            public EnemiesCardPair(Enemy enemy1, Enemy enemy2, string card)
                : base(new Enemy[] { enemy1, enemy2 }, card)
            {
            }

            public Enemy Enemy => this.First[0];

            public Enemy[] Enemies => this.First;

            public string Card => this.Second;
        }

        private class BestShotPair : Pair<string, BestShotHeader>
        {
            public BestShotPair(string name, BestShotHeader header)
                : base(name, header)
            {
            }

            public string Path => this.First;

            public BestShotHeader Header => this.Second;
        }

        private class AllScoreData
        {
            public AllScoreData() => this.Scores = new List<Score>(SpellCards.Count);

            public Header Header { get; private set; }

            public List<Score> Scores { get; private set; }

            public Status Status { get; private set; }

            public void Set(Header header) => this.Header = header;

            public void Set(Score score) => this.Scores.Add(score);

            public void Set(Status status) => this.Status = status;
        }

        private class Header : IBinaryReadable, IBinaryWritable
        {
            public const string ValidSignature = "T561";
            public const int SignatureSize = 4;
            public const int Size = SignatureSize + (sizeof(int) * 3) + (sizeof(uint) * 2);

            private uint unknown1;
            private uint unknown2;

            public Header()
            {
                this.Signature = string.Empty;
                this.EncodedAllSize = 0;
                this.EncodedBodySize = 0;
                this.DecodedBodySize = 0;
            }

            public string Signature { get; private set; }

            public int EncodedAllSize { get; private set; }

            public int EncodedBodySize { get; private set; }

            public int DecodedBodySize { get; private set; }

            public bool IsValid
            {
                get
                {
                    return this.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                        && (this.EncodedAllSize - this.EncodedBodySize == Size);
                }
            }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader is null)
                    throw new ArgumentNullException(nameof(reader));

                this.Signature = Encoding.Default.GetString(reader.ReadExactBytes(SignatureSize));
                this.EncodedAllSize = reader.ReadInt32();
                if (this.EncodedAllSize < 0)
                    throw new InvalidDataException(nameof(this.EncodedAllSize));
                this.unknown1 = reader.ReadUInt32();
                this.unknown2 = reader.ReadUInt32();
                this.EncodedBodySize = reader.ReadInt32();
                if (this.EncodedBodySize < 0)
                    throw new InvalidDataException(nameof(this.EncodedBodySize));
                this.DecodedBodySize = reader.ReadInt32();
                if (this.DecodedBodySize < 0)
                    throw new InvalidDataException(nameof(this.DecodedBodySize));
            }

            public void WriteTo(BinaryWriter writer)
            {
                if (writer is null)
                    throw new ArgumentNullException(nameof(writer));

                writer.Write(Encoding.Default.GetBytes(this.Signature));
                writer.Write(this.EncodedAllSize);
                writer.Write(this.unknown1);
                writer.Write(this.unknown2);
                writer.Write(this.EncodedBodySize);
                writer.Write(this.DecodedBodySize);
            }
        }

        private class Chapter : IBinaryReadable
        {
            public const int SignatureSize = 2;

            public Chapter()
            {
                this.Signature = string.Empty;
                this.Version = 0;
                this.Size = 0;
                this.Checksum = 0;
                this.Data = new byte[] { };
            }

            protected Chapter(Chapter chapter)
            {
                if (chapter is null)
                    throw new ArgumentNullException(nameof(chapter));

                this.Signature = chapter.Signature;
                this.Version = chapter.Version;
                this.Checksum = chapter.Checksum;
                this.Size = chapter.Size;
                this.Data = new byte[chapter.Data.Length];
                chapter.Data.CopyTo(this.Data, 0);
            }

            public string Signature { get; private set; }

            public ushort Version { get; private set; }

            public uint Checksum { get; private set; }

            public int Size { get; private set; }

            public bool IsValid
            {
                get
                {
                    var sum = BitConverter.GetBytes(this.Size).Sum(elem => elem);
                    sum += this.Data.Sum(elem => elem);
                    return (uint)sum == this.Checksum;
                }
            }

            protected byte[] Data { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader is null)
                    throw new ArgumentNullException(nameof(reader));

                this.Signature = Encoding.Default.GetString(reader.ReadExactBytes(SignatureSize));
                this.Version = reader.ReadUInt16();
                this.Checksum = reader.ReadUInt32();
                this.Size = reader.ReadInt32();
                this.Data = reader.ReadExactBytes(
                    this.Size - SignatureSize - sizeof(ushort) - sizeof(uint) - sizeof(int));
            }
        }

        private class Score : Chapter   // per scene
        {
            public const string ValidSignature = "SN";
            public const ushort ValidVersion = 0x0001;
            public const int ValidSize = 0x00000234;

            public Score(Chapter chapter)
                : base(chapter)
            {
                if (!this.Signature.Equals(ValidSignature, StringComparison.Ordinal))
                    throw new InvalidDataException(nameof(this.Signature));
                if (this.Version != ValidVersion)
                    throw new InvalidDataException(nameof(this.Version));
                if (this.Size != ValidSize)
                    throw new InvalidDataException(nameof(this.Size));

                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    this.Number = reader.ReadInt32();
                    this.ClearCount = reader.ReadInt32();
                    reader.ReadInt32(); // always same as ClearCount?
                    this.ChallengeCount = reader.ReadInt32();
                    this.NumPhotos = reader.ReadInt32();
                    this.HighScore = reader.ReadInt32();
                    reader.ReadExactBytes(0x210);   // always all 0x00?
                }
            }

            public int Number { get; }

            public int ClearCount { get; }

            public int ChallengeCount { get; }

            public int NumPhotos { get; }

            public int HighScore { get; }

            public static bool CanInitialize(Chapter chapter)
            {
                return chapter.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                    && (chapter.Version == ValidVersion)
                    && (chapter.Size == ValidSize);
            }
        }

        private class Status : Chapter
        {
            public const string ValidSignature = "ST";
            public const ushort ValidVersion = 0x0002;
            public const int ValidSize = 0x00000224;

            public Status(Chapter chapter)
                : base(chapter)
            {
                if (!this.Signature.Equals(ValidSignature, StringComparison.Ordinal))
                    throw new InvalidDataException(nameof(this.Signature));
                if (this.Version != ValidVersion)
                    throw new InvalidDataException(nameof(this.Version));
                if (this.Size != ValidSize)
                    throw new InvalidDataException(nameof(this.Size));

                using (var reader = new BinaryReader(new MemoryStream(this.Data, false)))
                {
                    this.LastName = reader.ReadExactBytes(14);
                    reader.ReadExactBytes(0x12);
                    this.BgmFlags = reader.ReadExactBytes(8);
                    reader.ReadExactBytes(0x18);
                    this.TotalPlayTime = reader.ReadInt32();
                    reader.ReadInt32(); // always 0?
                    reader.ReadInt32(); // 15?
                    reader.ReadInt32(); // always 0?
                    reader.ReadExactBytes(0x40);    // story flags?
                    this.NicknameFlags = reader.ReadExactBytes(51);
                    reader.ReadExactBytes(0x155);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public byte[] LastName { get; }         // .Length = 14 (The last 2 bytes are always 0x00 ?)

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public byte[] BgmFlags { get; }         // .Length = 8

            public int TotalPlayTime { get; }       // unit: [0.01s]

            public byte[] NicknameFlags { get; }    // .Length = 51 (The first byte should be ignored)

            public static bool CanInitialize(Chapter chapter)
            {
                return chapter.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                    && (chapter.Version == ValidVersion)
                    && (chapter.Size == ValidSize);
            }
        }

        private class BestShotHeader : IBinaryReadable
        {
            public const string ValidSignature = "BST4";
            public const int SignatureSize = 4;

            public string Signature { get; private set; }

            public Day Weekday { get; private set; }

            public short Dream { get; private set; } // 1-based

            public short Width { get; private set; }

            public short Height { get; private set; }

            public short Width2 { get; private set; }

            public short Height2 { get; private set; }

            public short HalfWidth { get; private set; }

            public short HalfHeight { get; private set; }

            // [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "For future use.")]
            public float SlowRate { get; private set; }

            public uint DateTime { get; private set; }

            public float Angle { get; private set; } // -PI .. +PI [rad]

            public int Score { get; private set; }

            public HashtagFields Fields { get; private set; }

            public int Score2 { get; private set; }

            public int BasePoint { get; private set; } // FIXME

            public int NumViewed { get; private set; }

            public int NumLikes { get; private set; }

            public int NumFavs { get; private set; }

            public int NumBullets { get; private set; }

            public int NumBulletsNearby { get; private set; }

            public int RiskBonus { get; private set; } // max(NumBulletsNearby, 2) * 40 .. min(NumBulletsNearby, 25) * 40

            public float BossShot { get; private set; } // 1.20? .. 2.00

            public float AngleBonus { get; private set; } // 1.00? .. 1.30

            public int MacroBonus { get; private set; } // 0 .. 60?

            public float LikesPerView { get; private set; }

            public float FavsPerView { get; private set; }

            public int NumHashtags { get; private set; }

            public int NumRedBullets { get; private set; }

            public int NumPurpleBullets { get; private set; }

            public int NumBlueBullets { get; private set; }

            public int NumCyanBullets { get; private set; }

            public int NumGreenBullets { get; private set; }

            public int NumYellowBullets { get; private set; }

            public int NumOrangeBullets { get; private set; }

            public int NumLightBullets { get; private set; }

            public void ReadFrom(BinaryReader reader)
            {
                if (reader is null)
                    throw new ArgumentNullException(nameof(reader));

                this.Signature = Encoding.Default.GetString(reader.ReadExactBytes(SignatureSize));
                if (!this.Signature.Equals(ValidSignature, StringComparison.Ordinal))
                    throw new InvalidDataException();

                reader.ReadUInt16();    // always 0x0401?
                this.Weekday = Utils.ToEnum<Day>(reader.ReadInt16());
                this.Dream = (short)(reader.ReadInt16() + 1);
                reader.ReadUInt16();    // 0x0100 ... Version?
                this.Width = reader.ReadInt16();
                this.Height = reader.ReadInt16();
                reader.ReadInt32(); // always 0?
                this.Width2 = reader.ReadInt16();
                this.Height2 = reader.ReadInt16();
                this.HalfWidth = reader.ReadInt16();
                this.HalfHeight = reader.ReadInt16();
                reader.ReadInt32(); // always 0?
                this.SlowRate = reader.ReadSingle();
                this.DateTime = reader.ReadUInt32();
                reader.ReadInt32(); // always 0?
                this.Angle = reader.ReadSingle();
                this.Score = reader.ReadInt32();
                reader.ReadInt32(); // always 0?
                this.Fields = new HashtagFields(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                reader.ReadBytes(0x28); // always all 0?
                this.Score2 = reader.ReadInt32();
                this.BasePoint = reader.ReadInt32();
                this.NumViewed = reader.ReadInt32();
                this.NumLikes = reader.ReadInt32();
                this.NumFavs = reader.ReadInt32();
                this.NumBullets = reader.ReadInt32();
                this.NumBulletsNearby = reader.ReadInt32();
                this.RiskBonus = reader.ReadInt32();
                this.BossShot = reader.ReadSingle();
                reader.ReadInt32(); // always 0? (Nice Shot?)
                this.AngleBonus = reader.ReadSingle();
                this.MacroBonus = reader.ReadInt32();
                reader.ReadInt32(); // always 0? (Front/Side/Back Shot?)
                reader.ReadInt32(); // always 0? (Clear Shot?)
                this.LikesPerView = reader.ReadSingle();
                this.FavsPerView = reader.ReadSingle();
                this.NumHashtags = reader.ReadInt32();
                this.NumRedBullets = reader.ReadInt32();
                this.NumPurpleBullets = reader.ReadInt32();
                this.NumBlueBullets = reader.ReadInt32();
                this.NumCyanBullets = reader.ReadInt32();
                this.NumGreenBullets = reader.ReadInt32();
                this.NumYellowBullets = reader.ReadInt32();
                this.NumOrangeBullets = reader.ReadInt32();
                this.NumLightBullets = reader.ReadInt32();
                reader.ReadBytes(0x44); // always all 0?
                reader.ReadBytes(0x34);
            }

            public struct HashtagFields
            {
                private static readonly int[] Masks;

                private readonly BitVector32[] data;

#pragma warning disable CA2207 // Initialize value type static fields inline
                static HashtagFields()
                {
                    Masks = new int[32];
                    Masks[0] = BitVector32.CreateMask();
                    for (var i = 1; i < Masks.Length; i++)
                    {
                        Masks[i] = BitVector32.CreateMask(Masks[i - 1]);
                    }
                }
#pragma warning restore CA2207 // Initialize value type static fields inline

                public HashtagFields(int data1, int data2, int data3)
                {
                    this.data = new BitVector32[3];
                    this.data[0] = new BitVector32(data1);
                    this.data[1] = new BitVector32(data2);
                    this.data[2] = new BitVector32(data3);
                }

                public bool EnemyIsInFrame => this.data[0][Masks[0]]; // Not used

                public bool EnemyIsPartlyInFrame => this.data[0][Masks[1]];

                public bool WholeEnemyIsInFrame => this.data[0][Masks[2]];

                public bool EnemyIsInMiddle => this.data[0][Masks[3]];

                public bool IsSelfie => this.data[0][Masks[4]];

                public bool IsTwoShot => this.data[0][Masks[5]];

                public bool BitDangerous => this.data[0][Masks[7]];

                public bool SeriouslyDangerous => this.data[0][Masks[8]];

                public bool ThoughtGonnaDie => this.data[0][Masks[9]];

                public bool ManyReds => this.data[0][Masks[10]];

                public bool ManyPurples => this.data[0][Masks[11]];

                public bool ManyBlues => this.data[0][Masks[12]];

                public bool ManyCyans => this.data[0][Masks[13]];

                public bool ManyGreens => this.data[0][Masks[14]];

                public bool ManyYellows => this.data[0][Masks[15]];

                public bool ManyOranges => this.data[0][Masks[16]];

                public bool TooColorful => this.data[0][Masks[17]];

                public bool SevenColors => this.data[0][Masks[18]];

                public bool NoBullet => this.data[0][Masks[19]]; // Not used

                public bool IsLandscapePhoto => this.data[0][Masks[21]];

                public bool Closeup => this.data[0][Masks[26]];

                public bool QuiteCloseup => this.data[0][Masks[27]];

                public bool TooClose => this.data[0][Masks[28]];

                public bool EnemyIsInFullView => this.data[1][Masks[1]];

                public bool TooManyBullets => this.data[1][Masks[4]];

                public bool TooPlayfulBarrage => this.data[1][Masks[5]];

                public bool TooDense => this.data[1][Masks[6]]; // FIXME

                public bool Chased => this.data[1][Masks[7]];

                public bool IsSuppository => this.data[1][Masks[8]];

                public bool IsButterflyLikeMoth => this.data[1][Masks[9]];

                public bool EnemyIsUndamaged => this.data[1][Masks[10]];

                public bool EnemyCanAfford => this.data[1][Masks[11]];

                public bool EnemyIsWeakened => this.data[1][Masks[12]];

                public bool EnemyIsDying => this.data[1][Masks[13]];

                public bool Finished => this.data[1][Masks[14]];

                public bool IsThreeShot => this.data[1][Masks[15]];

                public bool TwoEnemiesTogether => this.data[1][Masks[16]];

                public bool EnemiesAreOverlapping => this.data[1][Masks[17]];

                public bool PeaceSignAlongside => this.data[1][Masks[18]];

                public bool EnemiesAreTooClose => this.data[1][Masks[19]]; // FIXME

                public bool Scorching => this.data[1][Masks[20]];

                public bool TooBigBullet => this.data[1][Masks[21]];

                public bool ThrowingEdgedTools => this.data[1][Masks[22]];

                public bool Snaky => this.data[1][Masks[23]];

                public bool LightLooksStopped => this.data[1][Masks[24]];

                public bool IsSuperMoon => this.data[1][Masks[25]];

                public bool Dazzling => this.data[1][Masks[26]];

                public bool MoreDazzling => this.data[1][Masks[27]];

                public bool MostDazzling => this.data[1][Masks[28]];

                public bool FinishedTogether => this.data[1][Masks[29]];

                public bool WasDream => this.data[1][Masks[30]]; // FIXME; Not used

                public bool IsRockyBarrage => this.data[1][Masks[31]];

                public bool IsStickDestroyingBarrage => this.data[2][Masks[0]];

                public bool Fluffy => this.data[2][Masks[1]];

                public bool IsDoggiePhoto => this.data[2][Masks[2]];

                public bool IsAnimalPhoto => this.data[2][Masks[3]];

                public bool IsZoo => this.data[2][Masks[4]];

                public bool IsLovelyHeart => this.data[2][Masks[5]]; // FIXME

                public bool IsThunder => this.data[2][Masks[6]];

                public bool IsDrum => this.data[2][Masks[7]];

                public bool IsMisty => this.data[2][Masks[8]]; // FIXME

                public bool IsBoringPhoto => this.data[2][Masks[9]];

                public bool WasScolded => this.data[2][Masks[10]]; // FIXME

                public bool IsSumireko => this.data[2][Masks[11]];
            }
        }

        private class Hashtag
        {
            public Hashtag(bool outputs, string name)
            {
                this.Outputs = outputs;
                this.Name = name;
            }

            public bool Outputs { get; }

            public string Name { get; }
        }
    }
}