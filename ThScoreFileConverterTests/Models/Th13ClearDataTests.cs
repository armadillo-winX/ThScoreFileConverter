﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ThScoreFileConverter.Models;
using ThScoreFileConverterTests.Models.Wrappers;

namespace ThScoreFileConverterTests.Models
{
    [TestClass]
    public class Th13ClearDataTests
    {
        internal struct Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>
            where TChWithT : struct, Enum       // TCharaWithTotal
            where TLv : struct, Enum            // TLevel
            where TLvPrac : struct, Enum        // TLevelPractice
            where TLvPracWithT : struct, Enum   // TLevelPracticeWithTotal
            where TStPrac : struct, Enum        // TStagePractice
            where TStProg : struct, Enum        // TStageProgress
        {
            public string signature;
            public ushort version;
            public uint checksum;
            public int size;
            public TChWithT chara;
            public Dictionary<TLvPracWithT, Th10ScoreDataTests.Properties<TStProg>[]> rankings;
            public int totalPlayCount;
            public int playTime;
            public Dictionary<TLvPracWithT, int> clearCounts;
            public Dictionary<TLvPracWithT, int> clearFlags;
            public Dictionary<
                Th10LevelStagePairTests.Properties<TLvPrac, TStPrac>, Th13PracticeTests.Properties> practices;
            public Dictionary<int, Th13SpellCardTests.Properties<TLv>> cards;
        };

        internal static Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>
            GetValidProperties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                ushort version, int size, int numCards)
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
        {
            var levels = Utils.GetEnumerator<TLvPrac>();
            var levelsWithTotal = Utils.GetEnumerator<TLvPracWithT>();
            var stages = Utils.GetEnumerator<TStPrac>();

            return new Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>()
            {
                signature = "CR",
                version = version,
                checksum = 0u,
                size = size,
                chara = TestUtils.Cast<TChWithT>(1),
                rankings = levelsWithTotal.ToDictionary(
                    level => level,
                    level => Enumerable.Range(0, 10).Select(
                        index => new Th10ScoreDataTests.Properties<TStProg>()
                        {
                            score = 12345670u - (uint)index * 1000u,
                            stageProgress = TestUtils.Cast<TStProg>(5),
                            continueCount = (byte)index,
                            name = TestUtils.MakeRandomArray<byte>(10),
                            dateTime = 34567890u,
                            slowRate = 1.2f
                        }).ToArray()),
                totalPlayCount = 23,
                playTime = 4567890,
                clearCounts = levelsWithTotal.ToDictionary(level => level, level => 100 - TestUtils.Cast<int>(level)),
                clearFlags = levelsWithTotal.ToDictionary(level => level, level => TestUtils.Cast<int>(level) % 2),
                practices = levels
                    .SelectMany(level => stages.Select(stage => new { level, stage }))
                    .ToDictionary(
                        pair => new Th10LevelStagePairTests.Properties<TLvPrac, TStPrac>()
                        {
                            level = pair.level,
                            stage = pair.stage
                        },
                        pair => new Th13PracticeTests.Properties()
                        {
                            score = 123456u - TestUtils.Cast<uint>(pair.level) * 10u,
                            clearFlag = (byte)(TestUtils.Cast<int>(pair.stage) % 2),
                            enableFlag = (byte)(TestUtils.Cast<int>(pair.level) % 2)
                        }),
                cards = Enumerable.Range(1, numCards).ToDictionary(
                    index => index,
                    index => new Th13SpellCardTests.Properties<TLv>()
                    {
                        name = TestUtils.MakeRandomArray<byte>(0x80),
                        clearCount = 12 + index,
                        practiceClearCount = 34 + index,
                        trialCount = 56 + index,
                        practiceTrialCount = 78 + index,
                        id = index,
                        level = TestUtils.Cast<TLv>(2),
                        practiceScore = 90123
                    })
            };
        }

        internal static byte[] MakeData<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
            in Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg> properties)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.MakeByteArray(
                TestUtils.Cast<int>(properties.chara),
                properties.rankings.Values.SelectMany(
                    ranking => ranking.SelectMany(
                        scoreData => Th10ScoreDataTests.MakeByteArray<TParent, TStProg>(scoreData))).ToArray(),
                properties.totalPlayCount,
                properties.playTime,
                properties.clearCounts.Values.ToArray(),
                properties.clearFlags.Values.ToArray(),
                properties.practices.Values.SelectMany(
                    practice => Th13PracticeTests.MakeByteArray(practice)).ToArray(),
                properties.cards.Values.SelectMany(
                    card => Th13SpellCardTests.MakeByteArray(card)).ToArray());

        internal static byte[] MakeByteArray<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
            in Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg> properties)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.MakeByteArray(
                properties.signature.ToCharArray(),
                properties.version,
                properties.checksum,
                properties.size,
                MakeData<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties));

        internal static void Validate<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
            in Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg> clearData,
            in Properties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg> properties)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
        {
            var data = MakeData<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties);

            Assert.AreEqual(properties.signature, clearData.Signature);
            Assert.AreEqual(properties.version, clearData.Version);
            Assert.AreEqual(properties.checksum, clearData.Checksum);
            Assert.AreEqual(properties.size, clearData.Size);
            CollectionAssert.AreEqual(data, clearData.Data.ToArray());
            Assert.AreEqual(properties.chara, clearData.Chara);

            foreach (var pair in properties.rankings)
            {
                for (var index = 0; index < pair.Value.Length; ++index)
                {
                    Th10ScoreDataTests.Validate(clearData.RankingItem(pair.Key, index), pair.Value[index]);
                }
            }

            Assert.AreEqual(properties.totalPlayCount, clearData.TotalPlayCount);
            Assert.AreEqual(properties.playTime, clearData.PlayTime);
            CollectionAssert.AreEqual(properties.clearCounts.Values, clearData.ClearCounts.Values.ToArray());
            CollectionAssert.AreEqual(properties.clearFlags.Values, clearData.ClearFlags.Values.ToArray());

            foreach (var pair in properties.practices)
            {
                var levelStagePair = new Th10LevelStagePairWrapper<TParent, TLvPrac, TStPrac>(
                    pair.Key.level, pair.Key.stage);
                Th13PracticeTests.Validate(clearData.PracticesItem(levelStagePair), pair.Value);
            }

            foreach (var pair in properties.cards)
            {
                Th13SpellCardTests.Validate(clearData.CardsItem(pair.Key), pair.Value);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        internal static void
            ClearDataTestChapterHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                ushort version, int size, int numCards)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties =
                    GetValidProperties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(version, size, numCards);

                var chapter = Th10ChapterWrapper.Create(
                    MakeByteArray<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties));
                var clearData =
                    new Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(chapter);

                Validate(clearData, properties);
                Assert.IsFalse(clearData.IsValid.Value);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        internal static void
            ClearDataTestNullChapterHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>()
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var clearData =
                    new Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(null);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void
            ClearDataTestInvalidSignatureHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                ushort version, int size, int numCards)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties =
                    GetValidProperties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(version, size, numCards);
                properties.signature = properties.signature.ToLowerInvariant();

                var chapter = Th10ChapterWrapper.Create(
                    MakeByteArray<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties));
                var clearData =
                    new Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void
            ClearDataTestInvalidVersionHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                ushort version, int size, int numCards)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties =
                    GetValidProperties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(version, size, numCards);
                ++properties.version;

                var chapter = Th10ChapterWrapper.Create(
                    MakeByteArray<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties));
                var clearData =
                    new Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clearData")]
        internal static void
            ClearDataTestInvalidSizeHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                ushort version, int size, int numCards)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var properties =
                    GetValidProperties<TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(version, size, numCards);
                --properties.size;

                var chapter = Th10ChapterWrapper.Create(
                    MakeByteArray<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(properties));
                var clearData =
                    new Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(chapter);

                Assert.Fail(TestUtils.Unreachable);
            });

        internal static void
            CanInitializeTestHelper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>(
                string signature, ushort version, int size, bool expected)
            where TParent : ThConverter
            where TChWithT : struct, Enum
            where TLv : struct, Enum
            where TLvPrac : struct, Enum
            where TLvPracWithT : struct, Enum
            where TStPrac : struct, Enum
            where TStProg : struct, Enum
            => TestUtils.Wrap(() =>
            {
                var checksum = 0u;
                var data = new byte[size];

                var chapter = Th10ChapterWrapper.Create(
                    TestUtils.MakeByteArray(signature.ToCharArray(), version, checksum, size, data));

                Assert.AreEqual(
                    expected,
                    Th13ClearDataWrapper<TParent, TChWithT, TLv, TLvPrac, TLvPracWithT, TStPrac, TStProg>.CanInitialize(
                        chapter));
            });

        #region Th13

        [TestMethod]
        public void Th13ClearDataTestChapter()
            => ClearDataTestChapterHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>(1, 0x56DC, 127);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th13ClearDataTestNullChapter()
            => ClearDataTestNullChapterHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th13ClearDataTestInvalidSignature()
            => ClearDataTestInvalidSignatureHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>(1, 0x56DC, 127);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th13ClearDataTestInvalidVersion()
            => ClearDataTestInvalidVersionHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>(1, 0x56DC, 127);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th13ClearDataTestInvalidSize()
            => ClearDataTestInvalidSizeHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>(1, 0x56DC, 127);

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("CR", (ushort)1, 0x56DC, true)]
        [DataRow("cr", (ushort)1, 0x56DC, false)]
        [DataRow("CR", (ushort)0, 0x56DC, false)]
        [DataRow("CR", (ushort)1, 0x56DD, false)]
        public void Th13ClearDataCanInitializeTest(string signature, ushort version, int size, bool expected)
            => CanInitializeTestHelper<
                Th13Converter,
                Th13Converter.CharaWithTotal,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPractice,
                Th13Converter.LevelPracticeWithTotal,
                Th13Converter.StagePractice,
                Th13Converter.StageProgress>(
                signature, version, size, expected);

        #endregion

        #region Th14

        [TestMethod]
        public void Th14ClearDataTestChapter()
            => ClearDataTestChapterHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>(1, 0x5298, 120);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th14ClearDataTestNullChapter()
            => ClearDataTestNullChapterHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>();

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th14ClearDataTestInvalidSignature()
            => ClearDataTestInvalidSignatureHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>(1, 0x5298, 120);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th14ClearDataTestInvalidVersion()
            => ClearDataTestInvalidVersionHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>(1, 0x5298, 120);

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th14ClearDataTestInvalidSize()
            => ClearDataTestInvalidSizeHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>(1, 0x5298, 120);

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("CR", (ushort)1, 0x5298, true)]
        [DataRow("cr", (ushort)1, 0x5298, false)]
        [DataRow("CR", (ushort)0, 0x5298, false)]
        [DataRow("CR", (ushort)1, 0x5299, false)]
        public void Th14ClearDataCanInitializeTest(string signature, ushort version, int size, bool expected)
            => CanInitializeTestHelper<
                Th14Converter,
                Th14Converter.CharaWithTotal,
                ThConverter.Level,
                Th14Converter.LevelPractice,
                Th14Converter.LevelPracticeWithTotal,
                Th14Converter.StagePractice,
                Th14Converter.StageProgress>(
                signature, version, size, expected);

        #endregion
    }
}
