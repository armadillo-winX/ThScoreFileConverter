﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models.Wrappers
{
    // NOTE: Setting the accessibility as public causes CS0051 and CS0053.
    internal sealed class Th075StageEnemyPairWrapper
    {
        private static Type ParentType = typeof(Th075Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+StageEnemyPair";

        private readonly PrivateObject pobj = null;

        public Th075StageEnemyPairWrapper(Th075Converter.Stage stage, Th075Converter.Chara chara)
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest, new object[] { stage, chara });

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public object Target
            => this.pobj.Target;
        public Th075Converter.Stage? Stage
            => this.pobj.GetProperty(nameof(Stage)) as Th075Converter.Stage?;
        public Th075Converter.Chara? Enemy
            => this.pobj.GetProperty(nameof(Enemy)) as Th075Converter.Chara?;
    }
}
