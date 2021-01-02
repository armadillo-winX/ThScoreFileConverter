﻿//-----------------------------------------------------------------------
// <copyright file="CollectRateReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Helpers;

namespace ThScoreFileConverter.Models.Th105
{
    // %T105CRG[x][yy][z]
    internal class CollectRateReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T105CRG({0})({1})([1-2])", Parsers.LevelWithTotalParser.Pattern, Parsers.CharaParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public CollectRateReplacer(
            IReadOnlyDictionary<Chara, IClearData<Chara>> clearDataDictionary, INumberFormatter formatter)
        {
            this.evaluator = new MatchEvaluator(match =>
            {
                var level = Parsers.LevelWithTotalParser.Parse(match.Groups[1].Value);
                var chara = Parsers.CharaParser.Parse(match.Groups[2].Value);
                var type = IntegerHelper.Parse(match.Groups[3].Value);

                Func<KeyValuePair<(Chara, int), ISpellCardResult<Chara>>, bool> findByLevel = level switch
                {
                    LevelWithTotal.Total => FuncHelper.True,
                    _ => pair => pair.Value.Level == (Level)level,
                };

                Func<KeyValuePair<(Chara, int), ISpellCardResult<Chara>>, bool> countByType = type switch
                {
                    1 => pair => pair.Value.GotCount > 0,
                    _ => pair => pair.Value.TrialCount > 0,
                };

                var spellCardResults = clearDataDictionary.TryGetValue(chara, out var clearData)
                    ? clearData.SpellCardResults : ImmutableDictionary<(Chara, int), ISpellCardResult<Chara>>.Empty;
                return formatter.FormatNumber(spellCardResults.Where(findByLevel).Count(countByType));
            });
        }

        public string Replace(string input)
        {
            return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }
    }
}
