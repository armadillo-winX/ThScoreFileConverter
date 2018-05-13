﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ThScoreFileConverter.Models.Tests
{
    // NOTE: Setting the accessibility as public causes CS0051 and CS0053.
    internal sealed class Th07PlayCountWrapper
    {
        private static Type ParentType = typeof(Th07Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+PlayCount";

        private readonly PrivateObject pobj = null;

        public static Th07PlayCountWrapper Create(byte[] array)
        {
            var playCount = new Th07PlayCountWrapper();

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(array);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;
                    playCount.ReadFrom(reader);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return playCount;
        }

        public Th07PlayCountWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);
        public Th07PlayCountWrapper(object obj)
            => this.pobj = new PrivateObject(obj);

        // NOTE: Enabling the following causes CA1811.
        // public object Target => this.pobj.Target;


        public int? TotalTrial
            => this.pobj.GetProperty(nameof(TotalTrial)) as int?;
        public IReadOnlyDictionary<Th07Converter.Chara, int> Trials
            => this.pobj.GetProperty(nameof(Trials)) as Dictionary<Th07Converter.Chara, int>;
        public int? TotalRetry
            => this.pobj.GetProperty(nameof(TotalRetry)) as int?;
        public int? TotalClear
            => this.pobj.GetProperty(nameof(TotalClear)) as int?;
        public int? TotalContinue
            => this.pobj.GetProperty(nameof(TotalContinue)) as int?;
        public int? TotalPractice
            => this.pobj.GetProperty(nameof(TotalPractice)) as int?;

        public void ReadFrom(BinaryReader reader)
            => this.pobj.Invoke(nameof(ReadFrom), new object[] { reader }, CultureInfo.InvariantCulture);
    }
}