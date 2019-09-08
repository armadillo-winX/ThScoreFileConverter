﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter;
using ThScoreFileConverter.Models.Th07;
using ThScoreFileConverterTests.Models.Th06.Wrappers;

namespace ThScoreFileConverterTests.Models.Th07
{
    [TestClass]
    public class PracticeReplacerTests
    {
        internal static IReadOnlyDictionary<(Chara, Level, Stage), PracticeScore> PracticeScores { get; } =
            new List<PracticeScore>
            {
                new PracticeScore(ChapterWrapper.Create(
                    PracticeScoreTests.MakeByteArray(PracticeScoreTests.ValidProperties)).Target),
            }.ToDictionary(element => (element.Chara, element.Level, element.Stage));

        [TestMethod]
        public void PracticeReplacerTest()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.IsNotNull(replacer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PracticeReplacerTestNull()
        {
            _ = new PracticeReplacer(null);
            Assert.Fail(TestUtils.Unreachable);
        }

        [TestMethod]
        public void PracticeReplacerTestEmpty()
        {
            var practiceScores = new Dictionary<(Chara, Level, Stage), PracticeScore>();
            var replacer = new PracticeReplacer(practiceScores);
            Assert.IsNotNull(replacer);
        }

        [TestMethod]
        public void ReplaceTestScore()
        {
            var outputSeparator = Settings.Instance.OutputNumberGroupSeparator;

            var replacer = new PracticeReplacer(PracticeScores);

            Settings.Instance.OutputNumberGroupSeparator = true;
            Assert.AreEqual("1,234,560", replacer.Replace("%T07PRACHRB61"));

            Settings.Instance.OutputNumberGroupSeparator = false;
            Assert.AreEqual("1234560", replacer.Replace("%T07PRACHRB61"));

            Settings.Instance.OutputNumberGroupSeparator = outputSeparator;
        }

        [TestMethod]
        public void ReplaceTestTrialCount()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("987", replacer.Replace("%T07PRACHRB62"));
        }

        [TestMethod]
        public void ReplaceTestEmpty()
        {
            var practiceScores = new Dictionary<(Chara, Level, Stage), PracticeScore>();
            var replacer = new PracticeReplacer(practiceScores);
            Assert.AreEqual("0", replacer.Replace("%T07PRACHRB61"));
        }

        [TestMethod]
        public void ReplaceTestLevelExtra()
        {
            var practiceScores = new List<PracticeScore>
            {
                new PracticeScore(ChapterWrapper.Create(PracticeScoreTests.MakeByteArray(
                    new PracticeScoreTests.Properties
                    {
                        signature = PracticeScoreTests.ValidProperties.signature,
                        size1 = PracticeScoreTests.ValidProperties.size1,
                        size2 = PracticeScoreTests.ValidProperties.size2,
                        highScore = PracticeScoreTests.ValidProperties.highScore,
                        chara = PracticeScoreTests.ValidProperties.chara,
                        level = Level.Extra,
                        stage = PracticeScoreTests.ValidProperties.stage,
                    })).Target),
            }.ToDictionary(element => (element.Chara, element.Level, element.Stage));
            var replacer = new PracticeReplacer(practiceScores);
            Assert.AreEqual("%T07PRACXRB61", replacer.Replace("%T07PRACXRB61"));
        }

        [TestMethod]
        public void ReplaceTestStageExtra()
        {
            var practiceScores = new List<PracticeScore>
            {
                new PracticeScore(ChapterWrapper.Create(PracticeScoreTests.MakeByteArray(
                    new PracticeScoreTests.Properties
                    {
                        signature = PracticeScoreTests.ValidProperties.signature,
                        size1 = PracticeScoreTests.ValidProperties.size1,
                        size2 = PracticeScoreTests.ValidProperties.size2,
                        highScore = PracticeScoreTests.ValidProperties.highScore,
                        chara = PracticeScoreTests.ValidProperties.chara,
                        level = PracticeScoreTests.ValidProperties.level,
                        stage = Stage.Extra,
                    })).Target),
            }.ToDictionary(element => (element.Chara, element.Level, element.Stage));
            var replacer = new PracticeReplacer(practiceScores);
            Assert.AreEqual("%T07PRACHRBX1", replacer.Replace("%T07PRACHRBX1"));
        }

        [TestMethod]
        public void ReplaceTestLevelPhantasm()
        {
            var practiceScores = new List<PracticeScore>
            {
                new PracticeScore(ChapterWrapper.Create(PracticeScoreTests.MakeByteArray(
                    new PracticeScoreTests.Properties
                    {
                        signature = PracticeScoreTests.ValidProperties.signature,
                        size1 = PracticeScoreTests.ValidProperties.size1,
                        size2 = PracticeScoreTests.ValidProperties.size2,
                        highScore = PracticeScoreTests.ValidProperties.highScore,
                        chara = PracticeScoreTests.ValidProperties.chara,
                        level = Level.Phantasm,
                        stage = PracticeScoreTests.ValidProperties.stage,
                    })).Target),
            }.ToDictionary(element => (element.Chara, element.Level, element.Stage));
            var replacer = new PracticeReplacer(practiceScores);
            Assert.AreEqual("%T07PRACPRB61", replacer.Replace("%T07PRACPRB61"));
        }

        [TestMethod]
        public void ReplaceTestStagePhantasm()
        {
            var practiceScores = new List<PracticeScore>
            {
                new PracticeScore(ChapterWrapper.Create(PracticeScoreTests.MakeByteArray(
                    new PracticeScoreTests.Properties
                    {
                        signature = PracticeScoreTests.ValidProperties.signature,
                        size1 = PracticeScoreTests.ValidProperties.size1,
                        size2 = PracticeScoreTests.ValidProperties.size2,
                        highScore = PracticeScoreTests.ValidProperties.highScore,
                        chara = PracticeScoreTests.ValidProperties.chara,
                        level = PracticeScoreTests.ValidProperties.level,
                        stage = Stage.Phantasm,
                    })).Target),
            }.ToDictionary(element => (element.Chara, element.Level, element.Stage));
            var replacer = new PracticeReplacer(practiceScores);
            Assert.AreEqual("%T07PRACHRBP1", replacer.Replace("%T07PRACHRBP1"));
        }

        [TestMethod]
        public void ReplaceTestNonexistentLevel()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("0", replacer.Replace("%T07PRACNRB61"));
        }

        [TestMethod]
        public void ReplaceTestNonexistentChara()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("0", replacer.Replace("%T07PRACHRA61"));
        }

        [TestMethod]
        public void ReplaceTestNonexistentStage()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("0", replacer.Replace("%T07PRACHRB51"));
        }

        [TestMethod]
        public void ReplaceTestInvalidFormat()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("%T07XXXXHRB61", replacer.Replace("%T07XXXXHRB61"));
        }

        [TestMethod]
        public void ReplaceTestInvalidLevel()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("%T07PRACYRB61", replacer.Replace("%T07PRACYRB61"));
        }

        [TestMethod]
        public void ReplaceTestInvalidChara()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("%T07PRACHXX61", replacer.Replace("%T07PRACHXX61"));
        }

        [TestMethod]
        public void ReplaceTestInvalidStage()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("%T07PRACHRBY1", replacer.Replace("%T07PRACHRBY1"));
        }

        [TestMethod]
        public void ReplaceTestInvalidType()
        {
            var replacer = new PracticeReplacer(PracticeScores);
            Assert.AreEqual("%T07PRACHRB6X", replacer.Replace("%T07PRACHRB6X"));
        }
    }
}