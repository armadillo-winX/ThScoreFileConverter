﻿//-----------------------------------------------------------------------
// <copyright file="AllScoreData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.Linq;
using IClearData = ThScoreFileConverter.Models.Th06.IClearData<
    ThScoreFileConverter.Models.Th06.Chara,
    ThScoreFileConverter.Models.Level>;
using IHighScore = ThScoreFileConverter.Models.Th06.IHighScore<
    ThScoreFileConverter.Models.Th06.Chara,
    ThScoreFileConverter.Models.Level,
    ThScoreFileConverter.Models.Th06.StageProgress>;

namespace ThScoreFileConverter.Models.Th06
{
    internal class AllScoreData
    {
        private readonly Dictionary<(Chara, Level), IReadOnlyList<IHighScore>> rankings;
        private readonly Dictionary<Chara, IClearData> clearData;
        private readonly Dictionary<int, ICardAttack> cardAttacks;
        private readonly Dictionary<(Chara, Level, Stage), IPracticeScore> practiceScores;

        public AllScoreData()
        {
            var numCharas = Enum.GetValues(typeof(Chara)).Length;
            var numPairs = numCharas * Enum.GetValues(typeof(Level)).Length;
            var numTriples = numPairs * Enum.GetValues(typeof(Stage)).Length;
            this.rankings = new Dictionary<(Chara, Level), IReadOnlyList<IHighScore>>(numPairs);
            this.clearData = new Dictionary<Chara, IClearData>(numCharas);
            this.cardAttacks = new Dictionary<int, ICardAttack>(Definitions.CardTable.Count);
            this.practiceScores = new Dictionary<(Chara, Level, Stage), IPracticeScore>(numTriples);
        }

        public Header? Header { get; private set; }

        public IReadOnlyDictionary<(Chara, Level), IReadOnlyList<IHighScore>> Rankings => this.rankings;

        public IReadOnlyDictionary<Chara, IClearData> ClearData => this.clearData;

        public IReadOnlyDictionary<int, ICardAttack> CardAttacks => this.cardAttacks;

        public IReadOnlyDictionary<(Chara, Level, Stage), IPracticeScore> PracticeScores => this.practiceScores;

        public void Set(Header header)
        {
            this.Header = header;
        }

        public void Set(IHighScore score)
        {
            var key = (score.Chara, score.Level);
            if (!this.rankings.ContainsKey(key))
                this.rankings.Add(key, new List<IHighScore>(Definitions.InitialRanking));
            var ranking = this.rankings[key].ToList();
            ranking.Add(score);
            ranking.Sort((lhs, rhs) => rhs.Score.CompareTo(lhs.Score));
            ranking.RemoveAt(ranking.Count - 1);
            this.rankings[key] = ranking;
        }

        public void Set(IClearData data)
        {
            if (!this.clearData.ContainsKey(data.Chara))
                this.clearData.Add(data.Chara, data);
        }

        public void Set(ICardAttack attack)
        {
            if (!this.cardAttacks.ContainsKey(attack.CardId))
                this.cardAttacks.Add(attack.CardId, attack);
        }

        public void Set(IPracticeScore score)
        {
            if ((score.Level != Level.Extra) && (score.Stage != Stage.Extra) &&
                !((score.Level == Level.Easy) && (score.Stage == Stage.Six)))
            {
                var key = (score.Chara, score.Level, score.Stage);
                if (!this.practiceScores.ContainsKey(key))
                    this.practiceScores.Add(key, score);
            }
        }
    }
}
