﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    // NOTE: Setting the accessibility as public causes CS0050, CS0051 and CS0053.
    internal sealed class Th095AllScoreDataWrapper
    {
        private static Type ParentType = typeof(Th095Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+AllScoreData";

        private readonly PrivateObject pobj = null;

        public Th095AllScoreDataWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public object Target
            => this.pobj.Target;

        public Th095HeaderWrapper<Th095Converter> Header
        {
            get
            {
                var header = this.pobj.GetProperty(nameof(Header));
                return (header != null) ? new Th095HeaderWrapper<Th095Converter>(header) : null;
            }
        }

        // NOTE: Th095Converter.Score is a private class.
        // public IReadOnlyList<Score> Scores
        //     => this.pobj.GetProperty(nameof(Scores)) as List<Score>;
        public object Scores
            => this.pobj.GetProperty(nameof(Scores));
        public int? ScoresCount
            => this.Scores.GetType().GetProperty("Count").GetValue(this.Scores) as int?;
        public Th095ScoreWrapper ScoresItem(int index)
            => new Th095ScoreWrapper(
                this.Scores.GetType().GetProperty("Item").GetValue(this.Scores, new object[] { index }));

        public Th095StatusWrapper Status
        {
            get
            {
                var status = this.pobj.GetProperty(nameof(Status));
                return (status != null) ? new Th095StatusWrapper(status) : null;
            }
        }

        public void Set(Th095HeaderWrapper<Th095Converter> header)
            => this.pobj.Invoke(nameof(Set), new object[] { header.Target }, CultureInfo.InvariantCulture);
        public void Set(Th095ScoreWrapper data)
            => this.pobj.Invoke(nameof(Set), new object[] { data.Target }, CultureInfo.InvariantCulture);
        public void Set(Th095StatusWrapper status)
            => this.pobj.Invoke(nameof(Set), new object[] { status.Target }, CultureInfo.InvariantCulture);
    }
}