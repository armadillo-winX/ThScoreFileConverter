﻿//-----------------------------------------------------------------------
// <copyright file="PracticeReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static ThScoreFileConverter.Models.Th06.Parsers;

namespace ThScoreFileConverter.Models.Th06
{
    // %T06PRAC[x][yy][z]
    internal class PracticeReplacer : IStringReplaceable
    {
        private static readonly string Pattern = Utils.Format(
            @"%T06PRAC({0})({1})({2})", LevelParser.Pattern, CharaParser.Pattern, StageParser.Pattern);

        private readonly MatchEvaluator evaluator;

        public PracticeReplacer(IReadOnlyDictionary<(Chara, Level, Stage), IPracticeScore> practiceScores)
        {
            if (practiceScores is null)
                throw new ArgumentNullException(nameof(practiceScores));

            this.evaluator = new MatchEvaluator(match =>
            {
                var level = LevelParser.Parse(match.Groups[1].Value);
                var chara = CharaParser.Parse(match.Groups[2].Value);
                var stage = StageParser.Parse(match.Groups[3].Value);

                if (level == Level.Extra)
                    return match.ToString();
                if (stage == Stage.Extra)
                    return match.ToString();

                var key = (chara, level, stage);
                return Utils.ToNumberString(practiceScores.TryGetValue(key, out var score) ? score.HighScore : default);
            });
        }

        public string Replace(string input)
        {
            return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
        }
    }
}
