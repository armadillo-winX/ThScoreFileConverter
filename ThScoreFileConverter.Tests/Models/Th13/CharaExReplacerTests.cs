﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NSubstitute;
using ThScoreFileConverter.Core.Helpers;
using ThScoreFileConverter.Core.Models.Th13;
using ThScoreFileConverter.Models;
using ThScoreFileConverter.Models.Th13;
using IClearData = ThScoreFileConverter.Models.Th13.IClearData<
    ThScoreFileConverter.Core.Models.Th13.CharaWithTotal,
    ThScoreFileConverter.Core.Models.Th13.LevelPractice,
    ThScoreFileConverter.Core.Models.Th13.LevelPractice,
    ThScoreFileConverter.Core.Models.Th13.LevelPracticeWithTotal,
    ThScoreFileConverter.Core.Models.Th13.StagePractice,
    ThScoreFileConverter.Models.Th10.IScoreData<ThScoreFileConverter.Models.Th13.StageProgress>>;

namespace ThScoreFileConverter.Tests.Models.Th13;

[TestClass]
public class CharaExReplacerTests
{
    private static IEnumerable<IClearData> CreateClearDataList()
    {
        var levels = EnumHelper<LevelPracticeWithTotal>.Enumerable;

        var clearData1 = Substitute.For<IClearData>();
        _ = clearData1.Chara.Returns(CharaWithTotal.Marisa);
        _ = clearData1.TotalPlayCount.Returns(23);
        _ = clearData1.PlayTime.Returns(4567890);
        _ = clearData1.ClearCounts.Returns(levels.ToDictionary(level => level, level => 100 - (int)level));

        var clearData2 = Substitute.For<IClearData>();
        _ = clearData2.Chara.Returns(CharaWithTotal.Sanae);
        _ = clearData2.TotalPlayCount.Returns(12);
        _ = clearData2.PlayTime.Returns(3456789);
        _ = clearData2.ClearCounts.Returns(levels.ToDictionary(level => level, level => 50 - (int)level));

        return new[] { clearData1, clearData2 };
    }

    internal static IReadOnlyDictionary<CharaWithTotal, IClearData> ClearDataDictionary { get; } =
        CreateClearDataList().ToDictionary(clearData => clearData.Chara);

    private static INumberFormatter MockNumberFormatter()
    {
        // NOTE: NSubstitute v5.0.0 has no substitute for Moq's It.IsAny<It.IsValueType>.
        var mock = Substitute.For<INumberFormatter>();
        _ = mock.FormatNumber(Arg.Any<long>()).Returns(callInfo => $"invoked: {(long)callInfo[0]}");
        return mock;
    }

    [TestMethod]
    public void CharaExReplacerTest()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void CharaExReplacerTestEmpty()
    {
        var dictionary = ImmutableDictionary<CharaWithTotal, IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(dictionary, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ReplaceTestTotalPlayCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 23", replacer.Replace("%T13CHARAEXHMR1"));
    }

    [TestMethod]
    public void ReplaceTestPlayTime()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("21:08:51", replacer.Replace("%T13CHARAEXHMR2"));
    }

    [TestMethod]
    public void ReplaceTestClearCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 98", replacer.Replace("%T13CHARAEXHMR3"));
    }

    [TestMethod]
    public void ReplaceTestLevelTotalTotalPlayCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 23", replacer.Replace("%T13CHARAEXTMR1"));
    }

    [TestMethod]
    public void ReplaceTestLevelTotalPlayTime()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("21:08:51", replacer.Replace("%T13CHARAEXTMR2"));
    }

    [TestMethod]
    public void ReplaceTestLevelTotalClearCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 585", replacer.Replace("%T13CHARAEXTMR3"));
    }

    [TestMethod]
    public void ReplaceTestCharaTotalTotalPlayCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 35", replacer.Replace("%T13CHARAEXHTL1"));
    }

    [TestMethod]
    public void ReplaceTestCharaTotalPlayTime()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("37:09:04", replacer.Replace("%T13CHARAEXHTL2"));
    }

    [TestMethod]
    public void ReplaceTestCharaTotalClearCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 146", replacer.Replace("%T13CHARAEXHTL3"));
    }

    [TestMethod]
    public void ReplaceTestTotalTotalPlayCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 35", replacer.Replace("%T13CHARAEXTTL1"));
    }

    [TestMethod]
    public void ReplaceTestTotalPlayTime()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("37:09:04", replacer.Replace("%T13CHARAEXTTL2"));
    }

    [TestMethod]
    public void ReplaceTestTotalClearCount()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("invoked: 870", replacer.Replace("%T13CHARAEXTTL3"));
    }

    [TestMethod]
    public void ReplaceTestEmpty()
    {
        var dictionary = ImmutableDictionary<CharaWithTotal, IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(dictionary, formatterMock);
        Assert.AreEqual("invoked: 0", replacer.Replace("%T13CHARAEXHMR1"));
        Assert.AreEqual("0:00:00", replacer.Replace("%T13CHARAEXHMR2"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T13CHARAEXHMR3"));
    }

    [TestMethod]
    public void ReplaceTestEmptyClearCounts()
    {
        var clearData = Substitute.For<IClearData>();
        _ = clearData.Chara.Returns(CharaWithTotal.Marisa);
        _ = clearData.ClearCounts.Returns(ImmutableDictionary<LevelPracticeWithTotal, int>.Empty);
        var dictionary = new[] { clearData }.ToDictionary(data => data.Chara);
        var formatterMock = MockNumberFormatter();

        var replacer = new CharaExReplacer(dictionary, formatterMock);
        Assert.AreEqual("invoked: 0", replacer.Replace("%T13CHARAEXHMR3"));
    }

    [TestMethod]
    public void ReplaceTestInvalidFormat()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T13XXXXXXXHMR1", replacer.Replace("%T13XXXXXXXHMR1"));
    }

    [TestMethod]
    public void ReplaceTestInvalidLevel()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T13CHARAEXYMR1", replacer.Replace("%T13CHARAEXYMR1"));
    }

    [TestMethod]
    public void ReplaceTestInvalidChara()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T13CHARAEXHXX1", replacer.Replace("%T13CHARAEXHXX1"));
    }

    [TestMethod]
    public void ReplaceTestInvalidType()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new CharaExReplacer(ClearDataDictionary, formatterMock);
        Assert.AreEqual("%T13CHARAEXHMRX", replacer.Replace("%T13CHARAEXHMRX"));
    }
}
