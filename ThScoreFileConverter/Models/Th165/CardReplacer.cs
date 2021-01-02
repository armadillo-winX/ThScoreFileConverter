﻿//-----------------------------------------------------------------------
// <copyright file="CardReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Extensions;
using ThScoreFileConverter.Helpers;

namespace ThScoreFileConverter.Models.Th165
{
    // %T165CARD[xx][y][z]
    internal class CardReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T165CARD({0})([1-7])([12])", Parsers.DayParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public CardReplacer(IReadOnlyList<IScore> scores, bool hideUntriedCards)
        {
            this.evaluator = new MatchEvaluator(match =>
            {
                var day = Parsers.DayParser.Parse(match.Groups[1].Value);
                var scene = IntegerHelper.Parse(match.Groups[2].Value);
                var type = IntegerHelper.Parse(match.Groups[3].Value);

                var key = (day, scene);
                if (!Definitions.SpellCards.TryGetValue(key, out var enemyCardPair))
                    return match.ToString();

                if (hideUntriedCards)
                {
                    var score = scores.FirstOrDefault(elem =>
                        (elem is IScore) &&
                        (elem.Number >= 0) &&
                        (elem.Number < Definitions.SpellCards.Count) &&
                        Definitions.SpellCards.ElementAt(elem.Number).Key.Equals(key));
                    if ((score is null) || (score.ChallengeCount <= 0))
                        return "??????????";
                }

                if (type == 1)
                    return string.Join(" &amp; ", enemyCardPair.Enemies.Select(enemy => enemy.ToLongName()).ToArray());
                else
                    return enemyCardPair.Card;
            });
        }

        public string Replace(string input)
        {
            return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }
    }
}
