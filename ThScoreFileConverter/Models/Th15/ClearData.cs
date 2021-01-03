﻿//-----------------------------------------------------------------------
// <copyright file="ClearData.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThScoreFileConverter.Helpers;

namespace ThScoreFileConverter.Models.Th15
{
    internal class ClearData : Th10.Chapter, IClearData // per character
    {
        public const string ValidSignature = "CR";
        public const ushort ValidVersion = 0x0001;
        public const int ValidSize = 0x0000A4A0;

        public ClearData(Th10.Chapter chapter)
            : base(chapter, ValidSignature, ValidVersion, ValidSize)
        {
            using var stream = new MemoryStream(this.Data, false);
            using var reader = new BinaryReader(stream);

            this.Chara = (CharaWithTotal)reader.ReadInt32();

            this.GameModeData = EnumHelper<GameMode>.Enumerable.ToDictionary(mode => mode, _ =>
            {
                var data = new ClearDataPerGameMode();
                data.ReadFrom(reader);
                return data as IClearDataPerGameMode;
            });

            this.Practices = EnumHelper<Level>.Enumerable
                .SelectMany(level => EnumHelper<StagePractice>.Enumerable.Select(stage => (level, stage)))
                .ToDictionary(pair => pair, _ =>
                {
                    var practice = new Th13.Practice();
                    practice.ReadFrom(reader);
                    return practice as Th13.IPractice;
                });
        }

        public CharaWithTotal Chara { get; }

        public IReadOnlyDictionary<GameMode, IClearDataPerGameMode> GameModeData { get; }

        public IReadOnlyDictionary<(Level Level, StagePractice Stage), Th13.IPractice> Practices { get; }

        public static bool CanInitialize(Th10.Chapter chapter)
        {
            return chapter.Signature.Equals(ValidSignature, StringComparison.Ordinal)
                && (chapter.Version == ValidVersion)
                && (chapter.Size == ValidSize);
        }
    }
}
