﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter;
using ThScoreFileConverter.Models.Th095;

namespace ThScoreFileConverterTests.Models.Th095
{
    [TestClass]
    public class ScoreTotalReplacerTests
    {
        internal static IReadOnlyList<IScore> Scores { get; }

        static ScoreTotalReplacerTests()
        {
            var mock1 = ScoreTests.MockScore();

            var mock2 = ScoreTests.MockScore();
            _ = mock2.SetupGet(m => m.LevelScene).Returns((Level.Nine, 7));
            _ = mock2.SetupGet(m => m.HighScore).Returns(0);

            Scores = new[] { mock1.Object, mock2.Object };
        }

        [TestMethod]
        public void ScoreTotalReplacerTest()
        {
            var replacer = new ScoreTotalReplacer(Scores);
            Assert.IsNotNull(replacer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ScoreTotalReplacerTestNull()
        {
            _ = new ScoreTotalReplacer(null!);
            Assert.Fail(TestUtils.Unreachable);
        }

        [TestMethod]
        public void ScoreTotalReplacerTestEmpty()
        {
            var scores = new List<IScore>();
            var replacer = new ScoreTotalReplacer(scores);
            Assert.IsNotNull(replacer);
        }

        [TestMethod]
        public void ReplaceTestTotalScore()
        {
            var outputSeparator = Settings.Instance.OutputNumberGroupSeparator;

            var replacer = new ScoreTotalReplacer(Scores);

            Settings.Instance.OutputNumberGroupSeparator = true;
            Assert.AreEqual("1,234,567", replacer.Replace("%T95SCRTL1"));

            Settings.Instance.OutputNumberGroupSeparator = false;
            Assert.AreEqual("1234567", replacer.Replace("%T95SCRTL1"));

            Settings.Instance.OutputNumberGroupSeparator = outputSeparator;
        }

        [TestMethod]
        public void ReplaceTestTotalBestShotScore()
        {
            var outputSeparator = Settings.Instance.OutputNumberGroupSeparator;

            var replacer = new ScoreTotalReplacer(Scores);

            Settings.Instance.OutputNumberGroupSeparator = true;
            Assert.AreEqual("46,912", replacer.Replace("%T95SCRTL2"));

            Settings.Instance.OutputNumberGroupSeparator = false;
            Assert.AreEqual("46912", replacer.Replace("%T95SCRTL2"));

            Settings.Instance.OutputNumberGroupSeparator = outputSeparator;
        }

        [TestMethod]
        public void ReplaceTestTotalTrialCount()
        {
            var outputSeparator = Settings.Instance.OutputNumberGroupSeparator;

            var replacer = new ScoreTotalReplacer(Scores);

            Settings.Instance.OutputNumberGroupSeparator = true;
            Assert.AreEqual("19,752", replacer.Replace("%T95SCRTL3"));

            Settings.Instance.OutputNumberGroupSeparator = false;
            Assert.AreEqual("19752", replacer.Replace("%T95SCRTL3"));

            Settings.Instance.OutputNumberGroupSeparator = outputSeparator;
        }

        [TestMethod]
        public void ReplaceTestNumSucceededScenes()
        {
            var replacer = new ScoreTotalReplacer(Scores);
            Assert.AreEqual("1", replacer.Replace("%T95SCRTL4"));
        }

        [TestMethod]
        public void ReplaceTestEmpty()
        {
            var scores = new List<IScore>();
            var replacer = new ScoreTotalReplacer(scores);
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL1"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL2"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL3"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL4"));
        }

        [TestMethod]
        public void ReplaceTestNullScore()
        {
            var scores = new List<IScore>() { null! };
            var replacer = new ScoreTotalReplacer(scores);
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL1"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL2"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL3"));
            Assert.AreEqual("0", replacer.Replace("%T95SCRTL4"));
        }

        [TestMethod]
        public void ReplaceTestInvalidFormat()
        {
            var replacer = new ScoreTotalReplacer(Scores);
            Assert.AreEqual("%T95XXXXX1", replacer.Replace("%T95XXXXX1"));
        }

        [TestMethod]
        public void ReplaceTestInvalidType()
        {
            var replacer = new ScoreTotalReplacer(Scores);
            Assert.AreEqual("%T95SCRTLX", replacer.Replace("%T95SCRTLX"));
        }
    }
}
