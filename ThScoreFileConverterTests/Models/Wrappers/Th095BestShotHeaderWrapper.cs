﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using ThScoreFileConverter.Models;
using Level = ThScoreFileConverter.Models.Th095.Level;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    // NOTE: Setting the accessibility as public causes CS0053.
    internal sealed class Th095BestShotHeaderWrapper
    {
        private static readonly Type ParentType = typeof(Th095Converter);
        private static readonly string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static readonly string TypeNameToTest = ParentType.FullName + "+BestShotHeader";

        private readonly PrivateObject pobj = null;

        public static Th095BestShotHeaderWrapper Create(byte[] array)
        {
            var header = new Th095BestShotHeaderWrapper();

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(array);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;
                    header.ReadFrom(reader);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return header;
        }

        public Th095BestShotHeaderWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);
        public Th095BestShotHeaderWrapper(object original)
            => this.pobj = new PrivateObject(original);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public object Target
            => this.pobj.Target;
        public string Signature
            => this.pobj.GetProperty(nameof(this.Signature)) as string;
        public Level? Level
            => this.pobj.GetProperty(nameof(this.Level)) as Level?;
        public short? Scene
            => this.pobj.GetProperty(nameof(this.Scene)) as short?;
        public short? Width
            => this.pobj.GetProperty(nameof(this.Width)) as short?;
        public short? Height
            => this.pobj.GetProperty(nameof(this.Height)) as short?;
        public int? Score
            => this.pobj.GetProperty(nameof(this.Score)) as int?;
        public float? SlowRate
            => this.pobj.GetProperty(nameof(this.SlowRate)) as float?;
        public IReadOnlyCollection<byte> CardName
            => this.pobj.GetProperty(nameof(this.CardName)) as byte[];

        public void ReadFrom(BinaryReader reader)
            => this.pobj.Invoke(nameof(this.ReadFrom), new object[] { reader }, CultureInfo.InvariantCulture);
    }
}
