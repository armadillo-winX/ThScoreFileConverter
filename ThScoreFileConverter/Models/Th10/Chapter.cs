﻿//-----------------------------------------------------------------------
// <copyright file="Chapter.cs" company="None">
// Copyright (c) IIHOSHI Yoshinori.
// Licensed under the BSD-2-Clause license. See LICENSE.txt file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable SA1600 // Elements should be documented

namespace ThScoreFileConverter.Models.Th10
{
    using System;
    using System.IO;
    using System.Linq;

    internal class Chapter : IBinaryReadable
    {
        public const int SignatureSize = 2;

        public Chapter()
        {
            this.Signature = string.Empty;
            this.Version = 0;
            this.Checksum = 0;
            this.Size = 0;
            this.Data = new byte[] { };
        }

        protected Chapter(Chapter chapter)
        {
            if (chapter is null)
                throw new ArgumentNullException(nameof(chapter));

            this.Signature = chapter.Signature;
            this.Version = chapter.Version;
            this.Checksum = chapter.Checksum;
            this.Size = chapter.Size;
            this.Data = new byte[chapter.Data.Length];
            chapter.Data.CopyTo(this.Data, 0);
        }

        public string Signature { get; private set; }

        public ushort Version { get; private set; }

        public uint Checksum { get; private set; }

        public int Size { get; private set; }

        public bool IsValid
        {
            get
            {
                var sum = BitConverter.GetBytes(this.Size).Concat(this.Data).Sum(elem => (uint)elem);
                return sum == this.Checksum;
            }
        }

        protected byte[] Data { get; private set; }

        public void ReadFrom(BinaryReader reader)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            this.Signature = Encoding.Default.GetString(reader.ReadExactBytes(SignatureSize));
            this.Version = reader.ReadUInt16();
            this.Checksum = reader.ReadUInt32();
            this.Size = reader.ReadInt32();
            this.Data = reader.ReadExactBytes(
                this.Size - SignatureSize - sizeof(ushort) - sizeof(uint) - sizeof(int));
        }
    }
}