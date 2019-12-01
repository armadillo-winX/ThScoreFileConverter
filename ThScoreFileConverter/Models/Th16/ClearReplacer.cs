﻿//-----------------------------------------------------------------------
// <copyright file="ClearReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Extensions;

namespace ThScoreFileConverter.Models.Th16
{
    // %T16CLEAR[x][yy]
    internal class ClearReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T16CLEAR({0})({1})", Parsers.LevelParser.Pattern, Parsers.CharaParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public ClearReplacer(IReadOnlyDictionary<CharaWithTotal, IClearData> clearDataDictionary)
        {
            this.evaluator = new MatchEvaluator(match =>
            {
                var level = (LevelWithTotal)Parsers.LevelParser.Parse(match.Groups[1].Value);
                var chara = (CharaWithTotal)Parsers.CharaParser.Parse(match.Groups[2].Value);

                var rankings = clearDataDictionary[chara].Rankings[level]
                    .Where(ranking => ranking.DateTime > 0);
                var stageProgress = rankings.Any()
                    ? rankings.Max(ranking => ranking.StageProgress) : Th13.StageProgress.None;

                if (stageProgress == Th13.StageProgress.Extra)
                    return "Not Clear";
                else if (stageProgress == Th13.StageProgress.ExtraClear)
                    return Th13.StageProgress.Clear.ToShortName();
                else
                    return stageProgress.ToShortName();
            });
        }

        public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
    }
}
