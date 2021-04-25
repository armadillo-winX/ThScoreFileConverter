﻿using System.Collections.Generic;
using TemplateGenerator.Extensions;
using ThScoreFileConverter.Helpers;
using ThScoreFileConverter.Models.Th135;

namespace TemplateGenerator.Models.Th135
{
    public class Definitions
    {
        public static string Title { get; } = "東方心綺楼";

        public static IReadOnlyDictionary<string, string> LevelNames { get; } =
            EnumHelper<Level>.Enumerable.ToStringDictionary();

        public static IReadOnlyDictionary<string, string> CharacterNames { get; } = new[]
        {
            (Chara.Reimu,        "博麗 霊夢"),
            (Chara.Marisa,       "霧雨 魔理沙"),
            (Chara.IchirinUnzan, "雲居 一輪 &amp; 雲山"),
            (Chara.Byakuren,     "聖 白蓮"),
            (Chara.Futo,         "物部 布都"),
            (Chara.Miko,         "豊聡耳 神子"),
            (Chara.Nitori,       "河城 にとり"),
            (Chara.Koishi,       "古明地 こいし"),
            (Chara.Mamizou,      "二ッ岩 マミゾウ"),
            (Chara.Kokoro,       "秦 こころ"),
        }.ToStringKeyedDictionary();
    }
}
