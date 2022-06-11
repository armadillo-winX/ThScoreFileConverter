﻿//-----------------------------------------------------------------------
// <copyright file="Definitions.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace ThScoreFileConverter.Core.Models.Th165;

/// <summary>
/// Provides several VD specific definitions.
/// </summary>
public static class Definitions
{
    /// <summary>
    /// Gets the dictionary of VD spell cards.
    /// Thanks to thwiki.info.
    /// </summary>
    public static IReadOnlyDictionary<(Day Day, int Scene), (Enemy[] Enemies, string Card)> SpellCards { get; } =
        new Dictionary<(Day, int), (Enemy[], string)>
        {
            { (Day.Sunday,             1), (new[] { Enemy.Reimu },      string.Empty) },
            { (Day.Sunday,             2), (new[] { Enemy.Reimu },      string.Empty) },
            { (Day.Monday,             1), (new[] { Enemy.Seiran },     "弾符「イーグルシューティング」") },
            { (Day.Monday,             2), (new[] { Enemy.Ringo },      "兎符「ストロベリー大ダンゴ」") },
            { (Day.Monday,             3), (new[] { Enemy.Seiran },     "弾符「ラビットファルコナー」") },
            { (Day.Monday,             4), (new[] { Enemy.Ringo },      "兎符「ダンゴ三姉妹」") },
            { (Day.Tuesday,            1), (new[] { Enemy.Larva },      string.Empty) },
            { (Day.Tuesday,            2), (new[] { Enemy.Larva },      "蝶符「バタフライドリーム」") },
            { (Day.Tuesday,            3), (new[] { Enemy.Larva },      "蝶符「纏わり付く鱗粉」") },
            { (Day.Wednesday,          1), (new[] { Enemy.Marisa },     string.Empty) },
            { (Day.Wednesday,          2), (new[] { Enemy.Narumi },     "魔符「慈愛の地蔵」") },
            { (Day.Wednesday,          3), (new[] { Enemy.Narumi },     "地蔵「菩薩ストンプ」") },
            { (Day.Wednesday,          4), (new[] { Enemy.Narumi },     "地蔵「活きの良いバレットゴーレム」") },
            { (Day.Thursday,           1), (new[] { Enemy.Nemuno },     string.Empty) },
            { (Day.Thursday,           2), (new[] { Enemy.Nemuno },     "研符「狂い輝く鬼包丁」") },
            { (Day.Thursday,           3), (new[] { Enemy.Nemuno },     "殺符「窮僻の山姥」") },
            { (Day.Friday,             1), (new[] { Enemy.Aunn },       string.Empty) },
            { (Day.Friday,             2), (new[] { Enemy.Aunn },       "独楽「コマ犬大回転」") },
            { (Day.Friday,             3), (new[] { Enemy.Aunn },       "独楽「阿吽の閃光」") },
            { (Day.Saturday,           1), (new[] { Enemy.Doremy },     string.Empty) },
            { (Day.WrongSunday,        1), (new[] { Enemy.Reimu },      string.Empty) },
            { (Day.WrongSunday,        2), (new[] { Enemy.Seiran },     "夢弾「ルナティックドリームショット」") },
            { (Day.WrongSunday,        3), (new[] { Enemy.Ringo },      "団子「ダンゴフラワー」") },
            { (Day.WrongSunday,        4), (new[] { Enemy.Larva },      "夢蝶「クレージーバタフライ」") },
            { (Day.WrongSunday,        5), (new[] { Enemy.Narumi },     "夢地蔵「劫火の希望」") },
            { (Day.WrongSunday,        6), (new[] { Enemy.Nemuno },     "夢尽「殺人鬼の懐」") },
            { (Day.WrongSunday,        7), (new[] { Enemy.Aunn },       "夢犬「１０１匹の野良犬」") },
            { (Day.WrongMonday,        1), (new[] { Enemy.Clownpiece }, string.Empty) },
            { (Day.WrongMonday,        2), (new[] { Enemy.Clownpiece }, "獄符「バースティンググラッジ」") },
            { (Day.WrongMonday,        3), (new[] { Enemy.Clownpiece }, "獄符「ダブルストライプ」") },
            { (Day.WrongMonday,        4), (new[] { Enemy.Clownpiece }, "月夢「エクリプスナイトメア」") },
            { (Day.WrongTuesday,       1), (new[] { Enemy.Sagume },     string.Empty) },
            { (Day.WrongTuesday,       2), (new[] { Enemy.Sagume },     "玉符「金城鉄壁の陰陽玉」") },
            { (Day.WrongTuesday,       3), (new[] { Enemy.Sagume },     "玉符「神々の写し難い弾冠」") },
            { (Day.WrongTuesday,       4), (new[] { Enemy.Sagume },     "夢鷺「片翼の夢鷺」") },
            { (Day.WrongWednesday,     1), (new[] { Enemy.Doremy },     string.Empty) },
            { (Day.WrongWednesday,     2), (new[] { Enemy.Mai },        "竹符「バンブーラビリンス」") },
            { (Day.WrongWednesday,     3), (new[] { Enemy.Satono },     "茗荷「メスメリズムダンス」") },
            { (Day.WrongWednesday,     4), (new[] { Enemy.Mai },        "笹符「タナバタスタードリーム」") },
            { (Day.WrongWednesday,     5), (new[] { Enemy.Satono },     "冥加「ビハインドナイトメア」") },
            { (Day.WrongWednesday,     6), (new[] { Enemy.Mai, Enemy.Satono }, string.Empty) },
            { (Day.WrongThursday,      1), (new[] { Enemy.Hecatia },    "異界「ディストーテッドファイア」") },
            { (Day.WrongThursday,      2), (new[] { Enemy.Hecatia },    "異界「恨みがましい地獄の雨」") },
            { (Day.WrongThursday,      3), (new[] { Enemy.Hecatia },    "月「コズミックレディエーション」") },
            { (Day.WrongThursday,      4), (new[] { Enemy.Hecatia },    "異界「逢魔ガ刻　夢」") },
            { (Day.WrongThursday,      5), (new[] { Enemy.Hecatia },    "「月が堕ちてくる！」") },
            { (Day.WrongFriday,        1), (new[] { Enemy.Junko },      string.Empty) },
            { (Day.WrongFriday,        2), (new[] { Enemy.Junko },      "「震え凍える悪夢」") },
            { (Day.WrongFriday,        3), (new[] { Enemy.Junko },      "「サイケデリックマンダラ」") },
            { (Day.WrongFriday,        4), (new[] { Enemy.Junko },      "「極めて威厳のある純光」") },
            { (Day.WrongFriday,        5), (new[] { Enemy.Junko },      "「確実に悪夢で殺す為の弾幕」") },
            { (Day.WrongSaturday,      1), (new[] { Enemy.Okina },      "秘儀「マターラスッカ」") },
            { (Day.WrongSaturday,      2), (new[] { Enemy.Okina },      "秘儀「背面の邪炎」") },
            { (Day.WrongSaturday,      3), (new[] { Enemy.Okina },      "後符「絶対秘神の後光」") },
            { (Day.WrongSaturday,      4), (new[] { Enemy.Okina },      "秘儀「秘神の暗曜弾幕」") },
            { (Day.WrongSaturday,      5), (new[] { Enemy.Okina },      "秘儀「神秘の玉繭」") },
            { (Day.WrongSaturday,      6), (new[] { Enemy.Okina },      string.Empty) },
            { (Day.NightmareSunday,    1), (new[] { Enemy.Remilia,  Enemy.Flandre },      "紅魔符「ブラッディカタストロフ」") },
            { (Day.NightmareSunday,    2), (new[] { Enemy.Byakuren, Enemy.Miko },         "星神符「十七条の超人」") },
            { (Day.NightmareSunday,    3), (new[] { Enemy.Remilia,  Enemy.Byakuren },     "紅星符「超人ブラッディナイフ」") },
            { (Day.NightmareSunday,    4), (new[] { Enemy.Flandre,  Enemy.Miko },         "紅神符「十七条のカタストロフ」") },
            { (Day.NightmareSunday,    5), (new[] { Enemy.Remilia,  Enemy.Miko },         "神紅符「ブラッディ十七条のレーザー」") },
            { (Day.NightmareSunday,    6), (new[] { Enemy.Flandre,  Enemy.Byakuren },     "紅星符「超人カタストロフ行脚」") },
            { (Day.NightmareMonday,    1), (new[] { Enemy.Yuyuko,   Enemy.Eiki },         "妖花符「バタフライストーム閻魔笏」") },
            { (Day.NightmareMonday,    2), (new[] { Enemy.Kanako,   Enemy.Suwako },       "風神符「ミシャバシラ」") },
            { (Day.NightmareMonday,    3), (new[] { Enemy.Yuyuko,   Enemy.Kanako },       "風妖符「死蝶オンバシラ」") },
            { (Day.NightmareMonday,    4), (new[] { Enemy.Eiki,     Enemy.Suwako },       "風花符「ミシャグジ様の是非」") },
            { (Day.NightmareMonday,    5), (new[] { Enemy.Yuyuko,   Enemy.Suwako },       "妖風符「土着蝶ストーム」") },
            { (Day.NightmareMonday,    6), (new[] { Enemy.Eiki,     Enemy.Kanako },       "風花符「オンバシラ裁判」") },
            { (Day.NightmareTuesday,   1), (new[] { Enemy.Eirin,    Enemy.Kaguya },       "永夜符「蓬莱壺中の弾の枝」") },
            { (Day.NightmareTuesday,   2), (new[] { Enemy.Tenshi,   Enemy.Shinmyoumaru }, "緋針符「要石も大きくなあれ」") },
            { (Day.NightmareTuesday,   3), (new[] { Enemy.Eirin,    Enemy.Tenshi },       "永緋符「墜落する壺中の有頂天」") },
            { (Day.NightmareTuesday,   4), (new[] { Enemy.Kaguya,   Enemy.Shinmyoumaru }, "輝夜符「蓬莱の大きな弾の枝」") },
            { (Day.NightmareTuesday,   5), (new[] { Enemy.Eirin,    Enemy.Shinmyoumaru }, "永輝符「大きくなる壺」") },
            { (Day.NightmareTuesday,   6), (new[] { Enemy.Kaguya,   Enemy.Tenshi },       "緋夜符「蓬莱の弾の要石」") },
            { (Day.NightmareWednesday, 1), (new[] { Enemy.Satori,   Enemy.Utsuho },       "地霊符「マインドステラスチール」") },
            { (Day.NightmareWednesday, 2), (new[] { Enemy.Ran,      Enemy.Koishi },       "地妖符「イドの式神」") },
            { (Day.NightmareWednesday, 3), (new[] { Enemy.Satori,   Enemy.Koishi },       "「パーフェクトマインドコントロール」") },
            { (Day.NightmareWednesday, 4), (new[] { Enemy.Ran,      Enemy.Utsuho },       "地妖符「式神大星」") },
            { (Day.NightmareWednesday, 5), (new[] { Enemy.Ran,      Enemy.Satori },       "地妖符「エゴの式神」") },
            { (Day.NightmareWednesday, 6), (new[] { Enemy.Utsuho,   Enemy.Koishi },       "地霊符「マインドステラリリーフ」") },
            { (Day.NightmareThursday,  1), (new[] { Enemy.Nue,      Enemy.Mamizou },      "神星符「正体不明の怪光人だかり」") },
            { (Day.NightmareThursday,  2), (new[] { Enemy.Iku,      Enemy.Raiko },        "輝天符「迅雷のドンドコ太鼓」") },
            { (Day.NightmareThursday,  3), (new[] { Enemy.Mamizou,  Enemy.Raiko },        "輝神符「謎のドンドコ人だかり」") },
            { (Day.NightmareThursday,  4), (new[] { Enemy.Iku,      Enemy.Nue },          "緋星符「正体不明の落雷」") },
            { (Day.NightmareThursday,  5), (new[] { Enemy.Iku,      Enemy.Mamizou },      "神緋符「雷雨の中のストーカー」") },
            { (Day.NightmareThursday,  6), (new[] { Enemy.Nue,      Enemy.Raiko },        "輝星符「正体不明のドンドコ太鼓」") },
            { (Day.NightmareFriday,    1), (new[] { Enemy.Suika,    Enemy.Mokou },        "萃夜符「身命霧散」") },
            { (Day.NightmareFriday,    2), (new[] { Enemy.Junko,    Enemy.Hecatia },      "紺珠符「純粋と不純の弾幕」") },
            { (Day.NightmareFriday,    3), (new[] { Enemy.Suika,    Enemy.Junko },        "萃珠符「純粋な五里霧中」") },
            { (Day.NightmareFriday,    4), (new[] { Enemy.Mokou,    Enemy.Hecatia },      "永珠符「捨て身のリフレクション」") },
            { (Day.NightmareFriday,    5), (new[] { Enemy.Suika,    Enemy.Hecatia },      "萃珠符「ミストレイ」") },
            { (Day.NightmareFriday,    6), (new[] { Enemy.Mokou,    Enemy.Junko },        "永珠符「穢れ無き珠と穢れ多き霊」") },
            { (Day.NightmareSaturday,  1), (new[] { Enemy.Yukari,   Enemy.Okina },        "「秘神結界」") },
            { (Day.NightmareSaturday,  2), (new[] { Enemy.Reimu,    Enemy.Marisa },       "「盗撮者調伏マスタースパーク」") },
            { (Day.NightmareSaturday,  3), (new[] { Enemy.Reimu,    Enemy.Okina },        "「背後からの盗撮者調伏」") },
            { (Day.NightmareSaturday,  4), (new[] { Enemy.Marisa,   Enemy.Yukari },       "「弾幕結界を撃ち抜け！」") },
            { (Day.NightmareSaturday,  5), (new[] { Enemy.Marisa,   Enemy.Okina },        "「卑怯者マスタースパーク」") },
            { (Day.NightmareSaturday,  6), (new[] { Enemy.Reimu,    Enemy.Yukari },       "「許可無く弾幕は撮影禁止です」") },
            { (Day.NightmareDiary,     1), (new[] { Enemy.Doremy },                       "「最後の日曜日に見る悪夢」") },
            { (Day.NightmareDiary,     2), (new[] { Enemy.Sumireko },                     "紙符「ＥＳＰカード手裏剣」") },
            { (Day.NightmareDiary,     3), (new[] { Enemy.Sumireko, Enemy.Yukari },       "紙符「結界中のＥＳＰカード手裏剣」") },
            { (Day.NightmareDiary,     4), (new[] { Enemy.DreamSumireko },                string.Empty) },
        };
}
