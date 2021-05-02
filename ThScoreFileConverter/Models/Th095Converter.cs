﻿//-----------------------------------------------------------------------
// <copyright file="Th095Converter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // ElementsMustBeDocumented

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ThScoreFileConverter.Extensions;
using ThScoreFileConverter.Models.Th095;
using ThScoreFileConverter.Properties;

namespace ThScoreFileConverter.Models
{
#if !DEBUG
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Instantiated by ThConverterFactory.")]
#endif
    internal class Th095Converter : ThConverter
    {
        private readonly Dictionary<
            (Th095.Level Level, int Scene), (string Path, Th095.IBestShotHeader Header)> bestshots =
            new(Th095.Definitions.SpellCards.Count);

        private AllScoreData? allScoreData;

        public override string SupportedVersions { get; } = "1.02a";

        public override bool HasBestShotConverter { get; } = true;

        protected override bool ReadScoreFile(Stream input)
        {
            using var decrypted = new MemoryStream();
#if DEBUG
            using var decoded = new FileStream("th095decoded.dat", FileMode.Create, FileAccess.ReadWrite);
#else
            using var decoded = new MemoryStream();
#endif

            if (!Decrypt(input, decrypted))
                return false;

            decrypted.Seek(0, SeekOrigin.Begin);
            if (!Extract(decrypted, decoded))
                return false;

            decoded.Seek(0, SeekOrigin.Begin);
            if (!Validate(decoded))
                return false;

            decoded.Seek(0, SeekOrigin.Begin);
            this.allScoreData = Read(decoded);

            return this.allScoreData is not null;
        }

        protected override IEnumerable<IStringReplaceable> CreateReplacers(
            INumberFormatter formatter, bool hideUntriedCards, string outputFilePath)
        {
            if (this.allScoreData is null)
            {
                throw new InvalidDataException(
                    Utils.Format(Resources.InvalidOperationExceptionMustBeInvokedAfter, nameof(this.ReadScoreFile)));
            }

            return new List<IStringReplaceable>
            {
                new ScoreReplacer(this.allScoreData.Scores, formatter),
                new ScoreTotalReplacer(this.allScoreData.Scores, formatter),
                new CardReplacer(this.allScoreData.Scores, hideUntriedCards),
                new ShotReplacer(this.bestshots, formatter, outputFilePath),
                new ShotExReplacer(this.bestshots, this.allScoreData.Scores, formatter, outputFilePath),
            };
        }

        protected override string[] FilterBestShotFiles(string[] files)
        {
            var pattern = Utils.Format(@"bs_({0})_[1-9].dat", Parsers.LevelLongPattern);

            return files.Where(file => Regex.IsMatch(
                Path.GetFileName(file), pattern, RegexOptions.IgnoreCase)).ToArray();
        }

        protected override void ConvertBestShot(Stream input, Stream output)
        {
            using var decoded = new MemoryStream();

            if (output is not FileStream outputFile)
                throw new ArgumentException(Resources.ArgumentExceptionWrongType, nameof(output));

            var header = BestShotDeveloper.Develop<BestShotHeader>(input, output, PixelFormat.Format24bppRgb);

            var key = (header.Level, header.Scene);
            _ = this.bestshots.TryAdd(key, (outputFile.Name, header));
        }

        private static bool Decrypt(Stream input, Stream output)
        {
            using var reader = new BinaryReader(input, Encoding.UTF8, true);
            using var writer = new BinaryWriter(output, Encoding.UTF8, true);
            var header = new Header();

            header.ReadFrom(reader);
            if (!header.IsValid)
                return false;
            if (header.EncodedAllSize != reader.BaseStream.Length)
                return false;

            header.WriteTo(writer);
            ThCrypt.Decrypt(input, output, header.EncodedBodySize, 0xAC, 0x35, 0x10, header.EncodedBodySize);

            return true;
        }

        private static bool Extract(Stream input, Stream output)
        {
            using var reader = new BinaryReader(input, Encoding.UTF8, true);
            using var writer = new BinaryWriter(output, Encoding.UTF8, true);

            var header = new Header();
            header.ReadFrom(reader);
            header.WriteTo(writer);

            var bodyBeginPos = output.Position;
            Lzss.Decompress(input, output);
            output.Flush();
            output.SetLength(output.Position);

            return header.DecodedBodySize == (output.Position - bodyBeginPos);
        }

        private static bool Validate(Stream input)
        {
            using var reader = new BinaryReader(input, Encoding.UTF8, true);

            var header = new Header();
            header.ReadFrom(reader);
            var remainSize = header.DecodedBodySize;
            var chapter = new Chapter();

            try
            {
                while (remainSize > 0)
                {
                    chapter.ReadFrom(reader);
                    if (!chapter.IsValid)
                        return false;
                    if (!Score.CanInitialize(chapter) && !Status.CanInitialize(chapter))
                        return false;

                    remainSize -= chapter.Size;
                }
            }
            catch (EndOfStreamException)
            {
                // It's OK, do nothing.
            }

            return remainSize == 0;
        }

        private static AllScoreData? Read(Stream input)
        {
            var dictionary = new Dictionary<string, Action<AllScoreData, Chapter>>
            {
                { Score.ValidSignature,  (data, ch) => data.Set(new Score(ch))  },
                { Status.ValidSignature, (data, ch) => data.Set(new Status(ch)) },
            };

            using var reader = new BinaryReader(input, Encoding.UTF8, true);
            var allScoreData = new AllScoreData();
            var chapter = new Chapter();

            var header = new Header();
            header.ReadFrom(reader);
            allScoreData.Set(header);

            try
            {
                while (true)
                {
                    chapter.ReadFrom(reader);
                    if (dictionary.TryGetValue(chapter.Signature, out var setChapter))
                        setChapter(allScoreData, chapter);
                }
            }
            catch (EndOfStreamException)
            {
                // It's OK, do nothing.
            }

            if ((allScoreData.Header is not null) &&
                //// (allScoreData.scores.Count >= 0) &&
                (allScoreData.Status is not null))
                return allScoreData;
            else
                return null;
        }
    }
}
