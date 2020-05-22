﻿//-----------------------------------------------------------------------
// <copyright file="CharaExReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThScoreFileConverter.Models.Th15
{
    // %T15CHARAEX[w][x][yy][z]
    internal class CharaExReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T15CHARAEX({0})({1})({2})([1-3])",
            Parsers.GameModeParser.Pattern,
            Parsers.LevelWithTotalParser.Pattern,
            Parsers.CharaWithTotalParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public CharaExReplacer(IReadOnlyDictionary<CharaWithTotal, IClearData> clearDataDictionary)
        {
            if (clearDataDictionary is null)
                throw new ArgumentNullException(nameof(clearDataDictionary));

            this.evaluator = new MatchEvaluator(match =>
            {
                var mode = Parsers.GameModeParser.Parse(match.Groups[1].Value);
                var level = Parsers.LevelWithTotalParser.Parse(match.Groups[2].Value);
                var chara = Parsers.CharaWithTotalParser.Parse(match.Groups[3].Value);
                var type = int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                Func<IClearDataPerGameMode, long> getValueByType;
                Func<long, string> toString;
                if (type == 1)
                {
                    getValueByType = clearData => clearData.TotalPlayCount;
                    toString = Utils.ToNumberString;
                }
                else if (type == 2)
                {
                    getValueByType = clearData => clearData.PlayTime;
                    toString = value => new Time(value * 10, false).ToString();
                }
                else
                {
                    if (level == LevelWithTotal.Total)
                    {
                        getValueByType = clearData => clearData.ClearCounts.Values.Sum();
                    }
                    else
                    {
                        getValueByType =
                            clearData => clearData.ClearCounts.TryGetValue(level, out var count) ? count : default;
                    }

                    toString = Utils.ToNumberString;
                }

                Func<IReadOnlyDictionary<CharaWithTotal, IClearData>, long> getValueByChara;
                if (chara == CharaWithTotal.Total)
                {
                    getValueByChara = dictionary => dictionary.Values
                        .Where(clearData => clearData.Chara != chara)
                        .Sum(clearData => clearData.GameModeData.TryGetValue(mode, out var clearDataPerGameMode)
                            ? getValueByType(clearDataPerGameMode) : default);
                }
                else
                {
                    getValueByChara = dictionary => dictionary.TryGetValue(chara, out var clearData)
                        && clearData.GameModeData.TryGetValue(mode, out var clearDataPerGameMode)
                        ? getValueByType(clearDataPerGameMode) : default;
                }

                return toString(getValueByChara(clearDataDictionary));
            });
        }

        public string Replace(string input)
        {
            return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }
    }
}
