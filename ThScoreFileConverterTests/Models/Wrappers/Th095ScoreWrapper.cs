﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    // NOTE: Setting the accessibility as public causes CS0051 and CS0053.
    internal sealed class Th095ScoreWrapper
    {
        private static Type ParentType = typeof(Th095Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+Score";
        private static readonly PrivateType PrivateType = new PrivateType(AssemblyNameToTest, TypeNameToTest);

        private readonly PrivateObject pobj = null;

        public Th095ScoreWrapper(Th095ChapterWrapper<Th095Converter> chapter)
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest, new object[] { chapter?.Target });
        public Th095ScoreWrapper(object obj)
            => this.pobj = new PrivateObject(obj);

        public object Target
            => this.pobj.Target;
        public string Signature
            => this.pobj.GetProperty(nameof(this.Signature)) as string;
        public ushort? Version
            => this.pobj.GetProperty(nameof(this.Version)) as ushort?;
        public int? Size
            => this.pobj.GetProperty(nameof(this.Size)) as int?;
        public uint? Checksum
            => this.pobj.GetProperty(nameof(this.Checksum)) as uint?;
        public bool? IsValid
            => this.pobj.GetProperty(nameof(this.IsValid)) as bool?;
        public IReadOnlyCollection<byte> Data
            => this.pobj.GetProperty(nameof(this.Data)) as byte[];
        public Th095LevelScenePairWrapper<Th095Converter, Th095Converter.Level> LevelScene
            => new Th095LevelScenePairWrapper<Th095Converter, Th095Converter.Level>(
                this.pobj.GetProperty(nameof(this.LevelScene)));
        public int? HighScore
            => this.pobj.GetProperty(nameof(this.HighScore)) as int?;
        public int? BestshotScore
            => this.pobj.GetProperty(nameof(this.BestshotScore)) as int?;
        public uint? DateTime
            => this.pobj.GetProperty(nameof(this.DateTime)) as uint?;
        public int? TrialCount
            => this.pobj.GetProperty(nameof(this.TrialCount)) as int?;
        public float? SlowRate1
            => this.pobj.GetProperty(nameof(this.SlowRate1)) as float?;
        public float? SlowRate2
            => this.pobj.GetProperty(nameof(this.SlowRate2)) as float?;

        public static bool CanInitialize(Th095ChapterWrapper<Th095Converter> chapter)
            => (bool)PrivateType.InvokeStatic(
                nameof(CanInitialize), new object[] { chapter.Target }, CultureInfo.InvariantCulture);
    }
}