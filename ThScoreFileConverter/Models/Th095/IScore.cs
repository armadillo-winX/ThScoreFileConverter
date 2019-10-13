﻿//-----------------------------------------------------------------------
// <copyright file="IScore.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

namespace ThScoreFileConverter.Models.Th095
{
    internal interface IScore : IChapter
    {
        int BestshotScore { get; }

        uint DateTime { get; }

        int HighScore { get; }

        (Th095Converter.Level Level, int Scene) LevelScene { get; }

        float SlowRate1 { get; }

        float SlowRate2 { get; }

        int TrialCount { get; }
    }
}