﻿//-----------------------------------------------------------------------
// <copyright file="AllScoreData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;

namespace ThScoreFileConverter.Models.Th07
{
    internal class AllScoreData
    {
        public AllScoreData()
        {
            var numCharas = Enum.GetValues(typeof(Chara)).Length;
            var numPairs = numCharas * Enum.GetValues(typeof(Level)).Length;
            this.Rankings = new Dictionary<(Chara, Level), List<HighScore>>(numPairs);
            this.ClearData = new Dictionary<Chara, ClearData>(numCharas);
            this.CardAttacks = new Dictionary<int, CardAttack>(Definitions.CardTable.Count);
            this.PracticeScores = new Dictionary<(Chara, Level, Stage), PracticeScore>(numPairs);
        }

        public Header Header { get; private set; }

        public Dictionary<(Chara, Level), List<HighScore>> Rankings { get; private set; }

        public Dictionary<Chara, ClearData> ClearData { get; private set; }

        public Dictionary<int, CardAttack> CardAttacks { get; private set; }

        public Dictionary<(Chara, Level, Stage), PracticeScore> PracticeScores { get; private set; }

        public PlayStatus PlayStatus { get; private set; }

        public LastName LastName { get; private set; }

        public VersionInfo VersionInfo { get; private set; }

        public void Set(Header header) => this.Header = header;

        public void Set(HighScore score)
        {
            var key = (score.Chara, score.Level);
            if (!this.Rankings.ContainsKey(key))
                this.Rankings.Add(key, new List<HighScore>(Definitions.InitialRanking));
            var ranking = this.Rankings[key];
            ranking.Add(score);
            ranking.Sort((lhs, rhs) => rhs.Score.CompareTo(lhs.Score));
            ranking.RemoveAt(ranking.Count - 1);
        }

        public void Set(ClearData data)
        {
            if (!this.ClearData.ContainsKey(data.Chara))
                this.ClearData.Add(data.Chara, data);
        }

        public void Set(CardAttack attack)
        {
            if (!this.CardAttacks.ContainsKey(attack.CardId))
                this.CardAttacks.Add(attack.CardId, attack);
        }

        public void Set(PracticeScore score)
        {
            if ((score.Level != Level.Extra) && (score.Level != Level.Phantasm) &&
                (score.Stage != Stage.Extra) && (score.Stage != Stage.Phantasm))
            {
                var key = (score.Chara, score.Level, score.Stage);
                if (!this.PracticeScores.ContainsKey(key))
                    this.PracticeScores.Add(key, score);
            }
        }

        public void Set(PlayStatus status) => this.PlayStatus = status;

        public void Set(LastName name) => this.LastName = name;

        public void Set(VersionInfo info) => this.VersionInfo = info;
    }
}