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

namespace ThScoreFileConverter.Models.Th128
{
    // %T128CLEAR[x][yy]
    internal class ClearReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T128CLEAR({0})({1})", Parsers.LevelParser.Pattern, Parsers.RouteParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public ClearReplacer(IReadOnlyDictionary<RouteWithTotal, IClearData> clearDataDictionary)
        {
            this.evaluator = new MatchEvaluator(match =>
            {
                var level = Parsers.LevelParser.Parse(match.Groups[1].Value);
                var route = (RouteWithTotal)Parsers.RouteParser.Parse(match.Groups[2].Value);

                if ((level == Level.Extra) && (route != RouteWithTotal.Extra))
                    return match.ToString();
                if ((route == RouteWithTotal.Extra) && (level != Level.Extra))
                    return match.ToString();

                var rankings = clearDataDictionary[route].Rankings[level]
                    .Where(ranking => ranking.DateTime > 0);
                var stageProgress = rankings.Any()
                    ? rankings.Max(ranking => ranking.StageProgress) : StageProgress.None;

                return stageProgress.ToShortName();
            });
        }

        public string Replace(string input) => Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
    }
}
