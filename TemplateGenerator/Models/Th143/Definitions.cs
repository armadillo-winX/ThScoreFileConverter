﻿using System.Collections.Generic;
using System.Linq;
using TemplateGenerator.Extensions;
using ThScoreFileConverter.Core.Extensions;
using ThScoreFileConverter.Core.Models.Th143;
using ThScoreFileConverter.Models.Th143;

namespace TemplateGenerator.Models.Th143;

public class Definitions
{
    private static readonly IEnumerable<(ItemWithTotal, string)> ItemWithTotalNamesImpl = new[]
    {
        (ItemWithTotal.Fablic,   "布"),
        (ItemWithTotal.Camera,   "カメラ"),
        (ItemWithTotal.Umbrella, "傘"),
        (ItemWithTotal.Lantern,  "提灯"),
        (ItemWithTotal.Orb,      "陰陽玉"),
        (ItemWithTotal.Bomb,     "ボム"),
        (ItemWithTotal.Jizou,    "地蔵"),
        (ItemWithTotal.Doll,     "人形"),
        (ItemWithTotal.Mallet,   "小槌"),
        (ItemWithTotal.NoItem,   "未使用"),
        (ItemWithTotal.Total,    "合計"),
    };

    public static string Title { get; } = "弾幕アマノジャク";

    public static IReadOnlyDictionary<string, (string Id, string Name)> DayNames { get; } = new[]
    {
        (Day.First,   ("Day1",    "一日目")),
        (Day.Second,  ("Day2",    "二日目")),
        (Day.Third,   ("Day3",    "三日目")),
        (Day.Fourth,  ("Day4",    "四日目")),
        (Day.Fifth,   ("Day5",    "五日目")),
        (Day.Sixth,   ("Day6",    "六日目")),
        (Day.Seventh, ("Day7",    "七日目")),
        (Day.Eighth,  ("Day8",    "八日目")),
        (Day.Ninth,   ("Day9",    "九日目")),
        (Day.Last,    ("LastDay", "最終日")),
    }.ToStringKeyedDictionary();

    public static IReadOnlyDictionary<string, int> NumScenesPerDay { get; } = new[]
    {
        (Day.First,    6),
        (Day.Second,   6),
        (Day.Third,    7),
        (Day.Fourth,   7),
        (Day.Fifth,    8),
        (Day.Sixth,    8),
        (Day.Seventh,  8),
        (Day.Eighth,   7),
        (Day.Ninth,    8),
        (Day.Last,    10),
    }.ToStringKeyedDictionary();

    public static IReadOnlyDictionary<string, (string ShortName, string LongName)> ItemWithTotalNames { get; } =
        ItemWithTotalNamesImpl.ToDictionary(
            static pair => pair.Item1.ToShortName(),
            static pair => (pair.Item2, pair.Item1.ToLongName()));

    public static IReadOnlyDictionary<string, (string ShortName, string LongName)> ItemNames { get; } =
        ItemWithTotalNamesImpl.Where(static pair => pair.Item1 != ItemWithTotal.Total).ToDictionary(
            static pair => pair.Item1.ToShortName(),
            static pair => (pair.Item2, pair.Item1.ToLongName()));

    public static IEnumerable<string> ItemKeysTotalFirst { get; } = ItemWithTotalNames.Keys.RotateRight();

    public static IEnumerable<string> ItemKeysTotalLast { get; } = ItemWithTotalNames.Keys;

    public static int NumNicknames { get; } = 70;
}
