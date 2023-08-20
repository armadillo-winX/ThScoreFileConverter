﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NSubstitute;
using ThScoreFileConverter.Core.Extensions;
using ThScoreFileConverter.Core.Models.Th075;
using ThScoreFileConverter.Models.Th075;
using INumberFormatter = ThScoreFileConverter.Models.INumberFormatter;

namespace ThScoreFileConverter.Tests.Models.Th075;

[TestClass]
public class ScoreReplacerTests
{
    internal static IReadOnlyDictionary<(CharaWithReserved, Level), IClearData> ClearData { get; } =
        new Dictionary<(CharaWithReserved, Level), IClearData>
        {
            {
                (CharaWithReserved.Reimu, Level.Hard),
                ClearDataTests.MockClearData()
            },
        };

    private static INumberFormatter MockNumberFormatter()
    {
        // NOTE: NSubstitute v5.0.0 has no substitute for Moq's It.IsAny<It.IsValueType>.
        var mock = Substitute.For<INumberFormatter>();
        _ = mock.FormatNumber(Arg.Any<int>()).Returns(callInfo => $"invoked: {(int)callInfo[0]}");
        return mock;
    }

    [TestMethod]
    public void ScoreReplacerTest()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ScoreReplacerTestEmpty()
    {
        var clearData = ImmutableDictionary<(CharaWithReserved, Level), IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(clearData, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ScoreReplacerTestEmptyRanking()
    {
        var mock = ClearDataTests.MockClearData();
        _ = mock.Ranking.Returns(ImmutableList<IHighScore>.Empty);
        var clearData = new[] { ((CharaWithReserved.Reimu, Level.Hard), mock) }.ToDictionary();
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(clearData, formatterMock);
        Assert.IsNotNull(replacer);
    }

    [TestMethod]
    public void ReplaceTestName()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("Player0 ", replacer.Replace("%T75SCRHRM11"));
    }

    [TestMethod]
    public void ReplaceTestScore()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);

        Assert.AreEqual("invoked: 1234567", replacer.Replace("%T75SCRHRM12"));
    }

    [TestMethod]
    public void ReplaceTestDate()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("01/10", replacer.Replace("%T75SCRHRM13"));
    }

    [TestMethod]
    public void ReplaceTestEmpty()
    {
        var clearData = ImmutableDictionary<(CharaWithReserved, Level), IClearData>.Empty;
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(clearData, formatterMock);
        Assert.AreEqual(string.Empty, replacer.Replace("%T75SCRHRM11"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T75SCRHRM12"));
        Assert.AreEqual("00/00", replacer.Replace("%T75SCRHRM13"));
    }

    [TestMethod]
    public void ReplaceTestEmptyRanking()
    {
        var mock = ClearDataTests.MockClearData();
        _ = mock.Ranking.Returns(ImmutableList<IHighScore>.Empty);
        var clearData = new[] { ((CharaWithReserved.Reimu, Level.Hard), mock) }.ToDictionary();
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(clearData, formatterMock);
        Assert.AreEqual(string.Empty, replacer.Replace("%T75SCRHRM11"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T75SCRHRM12"));
        Assert.AreEqual("00/00", replacer.Replace("%T75SCRHRM13"));
    }

    [TestMethod]
    public void ReplaceTestMeiling()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75SCRHML11", replacer.Replace("%T75SCRHML11"));
    }

    [TestMethod]
    public void ReplaceTestNonexistentChara()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual(string.Empty, replacer.Replace("%T75SCRHMR11"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T75SCRHMR12"));
        Assert.AreEqual("00/00", replacer.Replace("%T75SCRHMR13"));
    }

    [TestMethod]
    public void ReplaceTestNonexistentLevel()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual(string.Empty, replacer.Replace("%T75SCRNRM11"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T75SCRNRM12"));
        Assert.AreEqual("00/00", replacer.Replace("%T75SCRNRM13"));
    }

    [TestMethod]
    public void ReplaceTestNonexistentRank()
    {
        var mock = ClearDataTests.MockClearData();
        var ranking = mock.Ranking;
        _ = mock.Ranking.Returns(ranking.Take(1).ToList());
        var clearData = new[] { ((CharaWithReserved.Reimu, Level.Hard), mock) }.ToDictionary();
        var formatterMock = MockNumberFormatter();

        var replacer = new ScoreReplacer(clearData, formatterMock);
        Assert.AreEqual(string.Empty, replacer.Replace("%T75SCRHRM21"));
        Assert.AreEqual("invoked: 0", replacer.Replace("%T75SCRHRM22"));
        Assert.AreEqual("00/00", replacer.Replace("%T75SCRHRM23"));
    }

    [TestMethod]
    public void ReplaceTestInvalidFormat()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75XXXHRM11", replacer.Replace("%T75XXXHRM11"));
    }

    [TestMethod]
    public void ReplaceTestInvalidLevel()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75SCRXRM11", replacer.Replace("%T75SCRXRM11"));
    }

    [TestMethod]
    public void ReplaceTestInvalidChara()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75SCRHXX11", replacer.Replace("%T75SCRHXX11"));
    }

    [TestMethod]
    public void ReplaceTestInvalidRank()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75SCRHRMX1", replacer.Replace("%T75SCRHRMX1"));
    }

    [TestMethod]
    public void ReplaceTestInvalidType()
    {
        var formatterMock = MockNumberFormatter();
        var replacer = new ScoreReplacer(ClearData, formatterMock);
        Assert.AreEqual("%T75SCRHRM1X", replacer.Replace("%T75SCRHRM1X"));
    }
}
