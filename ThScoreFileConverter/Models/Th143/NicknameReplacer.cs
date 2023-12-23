﻿//-----------------------------------------------------------------------
// <copyright file="NicknameReplacer.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Helpers;

namespace ThScoreFileConverter.Models.Th143;

// %T143NICK[xx]
internal sealed class NicknameReplacer(IStatus status) : IStringReplaceable
{
    private static readonly string Pattern = StringHelper.Create($@"{Definitions.FormatPrefix}NICK(\d{{2}})");

    private readonly MatchEvaluator evaluator = new(match =>
    {
        var number = IntegerHelper.Parse(match.Groups[1].Value);

        if ((number > 0) && (number <= Definitions.Nicknames.Count))
        {
            return (status.NicknameFlags.ElementAt(number) > 0)
                ? Definitions.Nicknames[number - 1] : "??????????";
        }
        else
        {
            return match.ToString();
        }
    });

    public string Replace(string input)
    {
        return Regex.Replace(input, Pattern, this.evaluator, RegexOptions.IgnoreCase);
    }
}
