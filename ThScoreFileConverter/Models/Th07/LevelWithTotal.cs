﻿//-----------------------------------------------------------------------
// <copyright file="LevelWithTotal.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace ThScoreFileConverter.Models.Th07
{
    /// <summary>
    /// Represents level of PCB and total.
    /// </summary>
    public enum LevelWithTotal
    {
        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("E")]
        Easy,

        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("N")]
        Normal,

        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("H")]
        Hard,

        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("L")]
        Lunatic,

        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("X")]
        Extra,

        /// <summary>
        /// Represents level Easy.
        /// </summary>
        [EnumAltName("P")]
        Phantasm,

        /// <summary>
        /// Represents total across levels.
        /// </summary>
        [EnumAltName("T")]
        Total,
    }
}