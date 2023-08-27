﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using NSubstitute;
using ThScoreFileConverter.Core.Helpers;
using ThScoreFileConverter.Core.Models.Th16;
using ThScoreFileConverter.Helpers;
using ThScoreFileConverter.Models;
using ThScoreFileConverter.Models.Th16;
using IClearData = ThScoreFileConverter.Models.Th13.IClearData<
    ThScoreFileConverter.Core.Models.Th16.CharaWithTotal,
    ThScoreFileConverter.Core.Models.Level,
    ThScoreFileConverter.Core.Models.Level,
    ThScoreFileConverter.Core.Models.Th14.LevelPracticeWithTotal,
    ThScoreFileConverter.Core.Models.Th14.StagePractice,
    ThScoreFileConverter.Models.Th16.IScoreData>;
using LevelPracticeWithTotal = ThScoreFileConverter.Core.Models.Th14.LevelPracticeWithTotal;
using StageProgress = ThScoreFileConverter.Models.Th13.StageProgress;

namespace ThScoreFileConverter.Tests.Models.Th16;

[TestClass]
public class ScoreReplacerTests
{
    internal static IReadOnlyDictionary<CharaWithTotal, IClearData> ClearDataDictionary { get; } =
        new[] { ClearDataTests.MockClearData() }.ToDictionary(clearData => clearData.Chara);

    private static INumberFormatter MockNumberFormatter()
    {
        // NOTE: NSubstitute v5.0.0 has no substitute for Moq's It.IsAny<It.IsValueType>.
        var mock = Substitute.For<INumberFormatter>();
        _ = mock.FormatNumber(Arg.Any<uint>()).Returns(callInfo => $"invoked: {(uint)callInfo[0]}");
        _ = mock.FormatPercent(Arg.Any<double>(), Arg.Any<int>())
            .Returns(callInfo => $"invoked: {((double)callInfo[0]).ToString($"F{(int)callInfo[1]}", CultureInfo.InvariantCulture)}%");
        return mock;
    }

    [TestMethod]
    public void ScoreReplacerTest()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ScoreReplacerTestEmpty()
    {
        var dictionary = ImmutableDictionary<CharaWithTotal, IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ReplaceTestName()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("Player1", replacer.Replace("%T16SCRHAY21"));
    }

    [TestMethod]
    public void ReplaceTestScore()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 123446701", replacer.Replace("%T16SCRHAY22"));
    }

    [TestMethod]
    public void ReplaceTestStage()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("Stage 5", replacer.Replace("%T16SCRHAY23"));
    }

    [TestMethod]
    public void ReplaceTestDateTime()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        var expected = DateTimeHelper.GetString(34567890);
        Assert.AreEqual(expected, replacer.Replace("%T16SCRHAY24"));
    }

    [TestMethod]
    public void ReplaceTestSlowRate()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 1.200%", replacer.Replace("%T16SCRHAY25"));  // really...?
    }

    [TestMethod]
    public void ReplaceTestSeason()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("秋", replacer.Replace("%T16SCRHAY26"));
    }

    [TestMethod]
    public void ReplaceTestEmpty()
    {
        var dictionary = ImmutableDictionary<CharaWithTotal, IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.AreEqual("--------", replacer.Replace("%T16SCRHAY21"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T16SCRHAY22"));
        Assert.AreEqual("-------", replacer.Replace("%T16SCRHAY23"));
        Assert.AreEqual(DateTimeHelper.GetString(null), replacer.Replace("%T16SCRHAY24"));
        Assert.AreEqual("-----%", replacer.Replace("%T16SCRHAY25"));
        Assert.AreEqual("-----", replacer.Replace("%T16SCRHAY26"));
    }

    [TestMethod]
    public void ReplaceTestEmptyRankings()
    {
        var clearData = Substitute.For<IClearData>();
        _ = clearData.Chara.Returns(CharaWithTotal.Aya);
        _ = clearData.Rankings.Returns(ImmutableDictionary<LevelPracticeWithTotal, IReadOnlyList<IScoreData>>.Empty);
        var dictionary = new[] { clearData }.ToDictionary(data => data.Chara);
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.AreEqual("--------", replacer.Replace("%T16SCRHAY21"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T16SCRHAY22"));
        Assert.AreEqual("-------", replacer.Replace("%T16SCRHAY23"));
        Assert.AreEqual(DateTimeHelper.GetString(null), replacer.Replace("%T16SCRHAY24"));
        Assert.AreEqual("-----%", replacer.Replace("%T16SCRHAY25"));
    }

    [TestMethod]
    public void ReplaceTestEmptyRanking()
    {
        var clearData = Substitute.For<IClearData>();
        _ = clearData.Chara.Returns(CharaWithTotal.Aya);
        _ = clearData.Rankings.Returns(
            EnumHelper<LevelPracticeWithTotal>.Enumerable.ToDictionary(
                level => level,
                level => ImmutableList<IScoreData>.Empty as IReadOnlyList<IScoreData>));
        var dictionary = new[] { clearData }.ToDictionary(data => data.Chara);
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.AreEqual("--------", replacer.Replace("%T16SCRHAY21"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T16SCRHAY22"));
        Assert.AreEqual("-------", replacer.Replace("%T16SCRHAY23"));
        Assert.AreEqual(DateTimeHelper.GetString(null), replacer.Replace("%T16SCRHAY24"));
        Assert.AreEqual("-----%", replacer.Replace("%T16SCRHAY25"));
    }

    [TestMethod]
    public void ReplaceTestStageExtra()
    {
        static IScoreData MockScoreData()
        {
            var mock = Substitute.For<IScoreData>();
            _ = mock.DateTime.Returns(34567890u);
            _ = mock.StageProgress.Returns(StageProgress.Extra);
            return mock;
        }

        static IClearData MockClearData()
        {
            var mock = Substitute.For<IClearData>();
            _ = mock.Chara.Returns(CharaWithTotal.Aya);
            _ = mock.Rankings.Returns(
                _ => EnumHelper<LevelPracticeWithTotal>.Enumerable.ToDictionary(
                    level => level,
                    level => Enumerable.Range(0, 10).Select(index => MockScoreData()).ToList()
                        as IReadOnlyList<IScoreData>));
            return mock;
        }

        var dictionary = new[] { MockClearData() }.ToDictionary(clearData => clearData.Chara);
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.AreEqual("Not Clear", replacer.Replace("%T16SCRHAY23"));
    }

    [TestMethod]
    public void ReplaceTestStageExtraClear()
    {
        static IScoreData CreateScoreData()
        {
            var mock = Substitute.For<IScoreData>();
            _ = mock.DateTime.Returns(34567890u);
            _ = mock.StageProgress.Returns(StageProgress.ExtraClear);
            return mock;
        }

        static IClearData CreateClearData()
        {
            var mock = Substitute.For<IClearData>();
            _ = mock.Chara.Returns(CharaWithTotal.Aya);
            _ = mock.Rankings.Returns(
                _ => EnumHelper<LevelPracticeWithTotal>.Enumerable.ToDictionary(
                    level => level,
                    level => Enumerable.Range(0, 10).Select(index => CreateScoreData()).ToList()
                        as IReadOnlyList<IScoreData>));
            return mock;
        }

        var dictionary = new[] { CreateClearData() }.ToDictionary(clearData => clearData.Chara);
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(dictionary, formatterMock);
        Assert.AreEqual("All Clear", replacer.Replace("%T16SCRHAY23"));
    }

    [TestMethod]
    public void ReplaceTestInvalidFormat()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T16XXXHAY21", replacer.Replace("%T16XXXHAY21"));
    }

    [TestMethod]
    public void ReplaceTestInvalidLevel()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T16SCRYAY21", replacer.Replace("%T16SCRYAY21"));
    }

    [TestMethod]
    public void ReplaceTestInvalidChara()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T16SCRHXX21", replacer.Replace("%T16SCRHXX21"));
    }

    [TestMethod]
    public void ReplaceTestInvalidRank()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T16SCRHAYX1", replacer.Replace("%T16SCRHAYX1"));
    }

    [TestMethod]
    public void ReplaceTestInvalidType()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T16SCRHAY2X", replacer.Replace("%T16SCRHAY2X"));
    }
}
