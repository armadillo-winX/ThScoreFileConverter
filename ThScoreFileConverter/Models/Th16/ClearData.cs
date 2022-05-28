﻿//-----------------------------------------------------------------------
// <copyright file="ClearData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using ClearDataBase = ThScoreFileConverter.Models.Th16.ClearDataBase<
    ThScoreFileConverter.Core.Models.Th16.CharaWithTotal,
    ThScoreFileConverter.Core.Models.Level,
    ThScoreFileConverter.Core.Models.Level,
    ThScoreFileConverter.Models.Th14.LevelPracticeWithTotal,
    ThScoreFileConverter.Models.Th14.StagePractice,
    ThScoreFileConverter.Models.Th16.IScoreData,
    ThScoreFileConverter.Models.Th16.ScoreData>;

namespace ThScoreFileConverter.Models.Th16;

internal class ClearData : ClearDataBase    // per character
{
    public const ushort ValidVersion = 0x0001;
    public const int ValidSize = 0x00005318;

    public ClearData(Th10.Chapter chapter)
        : base(chapter, ValidVersion, ValidSize, Definitions.CardTable.Count)
    {
    }

    public static new bool CanInitialize(Th10.Chapter chapter)
    {
        return ClearDataBase.CanInitialize(chapter)
            && (chapter.Version == ValidVersion)
            && (chapter.Size == ValidSize);
    }
}
