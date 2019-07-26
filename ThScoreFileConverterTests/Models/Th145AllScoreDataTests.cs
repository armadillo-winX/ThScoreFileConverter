﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using ThScoreFileConverter.Models;
using ThScoreFileConverterTests.Models.Wrappers;

namespace ThScoreFileConverterTests.Models
{
    using SQOT = ThScoreFileConverter.Squirrel.SQObjectType;

    [TestClass]
    public class Th145AllScoreDataTests
    {
        internal struct Properties
        {
            public int storyProgress;
            public Dictionary<Th145Converter.Chara, Th145Converter.LevelFlag> storyClearFlags;
            public int endingCount;
            public int ending2Count;
            public bool isEnabledStageTanuki1;
            public bool isEnabledStageTanuki2;
            public bool isEnabledStageKokoro;
            public bool isEnabledSt27;
            public bool isEnabledSt28;
            public bool isPlayableMamizou;
            public bool isPlayableKokoro;
            public Dictionary<int, bool> bgmFlags;
            public Dictionary<Th145Converter.Level, Dictionary<Th145Converter.Chara, int>> clearRanks;
            public Dictionary<Th145Converter.Level, Dictionary<Th145Converter.Chara, int>> clearTimes;
        };

        internal static Properties GetValidProperties()
        {
            var charas = Utils.GetEnumerator<Th145Converter.Chara>();
            var levels = Utils.GetEnumerator<Th145Converter.Level>();

            return new Properties()
            {
                storyProgress = 1,
                storyClearFlags = charas.ToDictionary(
                    chara => chara, chara => TestUtils.Cast<Th145Converter.LevelFlag>(30 - (int)chara)),
                endingCount = 2,
                ending2Count = 3,
                isEnabledStageTanuki1 = true,
                isEnabledStageTanuki2 = true,
                isEnabledStageKokoro = false,
                isEnabledSt27 = false,
                isEnabledSt28 = false,
                isPlayableMamizou = true,
                isPlayableKokoro = false,
                bgmFlags = Enumerable.Range(1, 10).ToDictionary(id => id, id => id % 2 == 0),
                clearRanks = levels.ToDictionary(
                    level => level, level => charas.ToDictionary(
                        chara => chara, chara => (int)level * 100 + (int)chara)),
                clearTimes = levels.ToDictionary(
                    level => level, level => charas.ToDictionary(
                        chara => chara, chara => (int)chara * 100 + (int)level))
            };
        }

        internal static byte[] MakeByteArray(in Properties properties)
            => new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table))
                .Concat(TestUtils.MakeSQByteArray(
                    "story_progress", properties.storyProgress,
                    "story_clear", properties.storyClearFlags.Select(pair => (int)pair.Value).ToArray(),
                    "ed_count", properties.endingCount,
                    "ed2_count", properties.ending2Count,
                    "enable_stage_tanuki1", properties.isEnabledStageTanuki1,
                    "enable_stage_tanuki2", properties.isEnabledStageTanuki2,
                    "enable_stage_kokoro", properties.isEnabledStageKokoro,
                    "enable_st27", properties.isEnabledSt27,
                    "enable_st28", properties.isEnabledSt28,
                    "enable_mamizou", properties.isPlayableMamizou,
                    "enable_kokoro", properties.isPlayableKokoro,
                    "enable_bgm", properties.bgmFlags,
                    "clear_rank", properties.clearRanks.Select(
                        perLevelPair => perLevelPair.Value.Select(
                            perCharaPair => perCharaPair.Value).ToArray()).ToArray(),
                    "clear_time", properties.clearTimes.Select(
                        perLevelPair => perLevelPair.Value.Select(
                            perCharaPair => perCharaPair.Value).ToArray()).ToArray()))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray();

        internal static void Validate(in Th145AllScoreDataWrapper allScoreData, in Properties properties)
        {
            Assert.AreEqual(properties.storyProgress, allScoreData.StoryProgress);
            CollectionAssert.AreEqual(properties.storyClearFlags.Keys, allScoreData.StoryClearFlags.Keys.ToArray());
            CollectionAssert.AreEqual(properties.storyClearFlags.Values, allScoreData.StoryClearFlags.Values.ToArray());
            Assert.AreEqual(properties.endingCount, allScoreData.EndingCount);
            Assert.AreEqual(properties.ending2Count, allScoreData.Ending2Count);
            Assert.AreEqual(properties.isEnabledStageTanuki1, allScoreData.IsEnabledStageTanuki1);
            Assert.AreEqual(properties.isEnabledStageTanuki2, allScoreData.IsEnabledStageTanuki2);
            Assert.AreEqual(properties.isEnabledStageKokoro, allScoreData.IsEnabledStageKokoro);
            Assert.AreEqual(properties.isEnabledSt27, allScoreData.IsEnabledSt27);
            Assert.AreEqual(properties.isEnabledSt28, allScoreData.IsEnabledSt28);
            Assert.AreEqual(properties.isPlayableMamizou, allScoreData.IsPlayableMamizou);
            Assert.AreEqual(properties.isPlayableKokoro, allScoreData.IsPlayableKokoro);
            CollectionAssert.AreEqual(properties.bgmFlags.Keys, allScoreData.BgmFlags.Keys.ToArray());
            CollectionAssert.AreEqual(properties.bgmFlags.Values, allScoreData.BgmFlags.Values.ToArray());
            CollectionAssert.AreEqual(properties.clearRanks.Keys, allScoreData.ClearRanks.Keys.ToArray());

            foreach (var pair in properties.clearRanks)
            {
                CollectionAssert.AreEqual(pair.Value.Keys, allScoreData.ClearRanks[pair.Key].Keys);
                CollectionAssert.AreEqual(pair.Value.Values, allScoreData.ClearRanks[pair.Key].Values);
            }

            CollectionAssert.AreEqual(properties.clearTimes.Keys, allScoreData.ClearTimes.Keys.ToArray());

            foreach (var pair in properties.clearTimes)
            {
                CollectionAssert.AreEqual(pair.Value.Keys, allScoreData.ClearTimes[pair.Key].Keys);
                CollectionAssert.AreEqual(pair.Value.Values, allScoreData.ClearTimes[pair.Key].Values);
            }
        }

        internal static bool Th145AllScoreDataReadObjectHelper(byte[] array, out object obj)
        {
            var result = false;

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(array);
                using (var reader = new BinaryReader(stream))
                {
                    stream = null;
                    result = Th145AllScoreDataWrapper.ReadObject(reader, out obj);
                }
            }
            finally
            {
                stream?.Dispose();
            }

            return result;
        }

        [TestMethod]
        public void Th145AllScoreDataTest() => TestUtils.Wrap(() =>
        {
            var allScoreData = new Th145AllScoreDataWrapper();

            Assert.AreEqual(default, allScoreData.StoryProgress.Value);
            Assert.IsNull(allScoreData.StoryClearFlags);
            Assert.AreEqual(default, allScoreData.EndingCount.Value);
            Assert.AreEqual(default, allScoreData.Ending2Count.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageTanuki1.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageTanuki2.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageKokoro.Value);
            Assert.AreEqual(default, allScoreData.IsPlayableMamizou.Value);
            Assert.AreEqual(default, allScoreData.IsPlayableKokoro.Value);
            Assert.IsNull(allScoreData.BgmFlags);
        });

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th145AllScoreDataReadObjectTestNull() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataWrapper.ReadObject(null, out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestEmpty() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(new byte[0], out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        public void Th145AllScoreDataReadObjectTestOTNull() => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)SQOT.Null), out var obj);

            Assert.IsTrue(result);
            Assert.IsNotNull(obj);  // Hmm...
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-1)]
        public void Th145AllScoreDataReadObjectTestOTInteger(int value) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeSQByteArray(value).ToArray(), out var obj);

            Assert.IsTrue(result);
            Assert.IsTrue(obj is int);
            Assert.AreEqual(value, (int)obj);
        });

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTIntegerInvalid() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)SQOT.Integer, new byte[3]), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(0f)]
        [DataRow(1f)]
        [DataRow(-1f)]
        [DataRow(0.25f)]
        [DataRow(0.1f)]
        public void Th145AllScoreDataReadObjectTestOTFloat(float value) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeSQByteArray(value).ToArray(), out var obj);

            Assert.IsTrue(result);
            Assert.IsTrue(obj is float);
            Assert.AreEqual(value, (float)obj);
        });

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTFloatInvalid() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)SQOT.Float, new byte[3]), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow((byte)0x00, false)]
        [DataRow((byte)0x01, true)]
        [DataRow((byte)0x02, true)]
        public void Th145AllScoreDataReadObjectTestOTBool(byte value, bool expected) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(
                TestUtils.MakeByteArray((int)SQOT.Bool, value), out var obj);

            Assert.IsTrue(result);
            Assert.IsTrue(obj is bool);
            Assert.AreEqual(expected, (bool)obj);
        });

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTBoolInvalid() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)SQOT.Bool), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(0, "", "")]
        [DataRow(0, "abc", "")]
        [DataRow(0, null, "")]
        [DataRow(-1, "", "")]
        [DataRow(-1, "abc", "")]
        [DataRow(-1, null, "")]
        public void Th145AllScoreDataReadObjectTestOTStringEmpty(int size, string value, string expected)
            => TestUtils.Wrap(() =>
            {
                var bytes = (value != null) ? TestUtils.CP932Encoding.GetBytes(value) : new byte[0];
                var result = Th145AllScoreDataReadObjectHelper(
                    TestUtils.MakeByteArray((int)SQOT.String, size, bytes), out var obj);
                var str = obj as string;

                Assert.IsTrue(result);
                Assert.IsNotNull(str);
                Assert.AreEqual(expected, str);
            });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("abc")]
        [DataRow("博麗 霊夢")]
        [DataRow("")]
        [DataRow("\0")]
        public void Th145AllScoreDataReadObjectTestOTString(string value) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeSQByteArray(value).ToArray(), out var obj);
            var str = obj as string;

            Assert.IsTrue(result);
            Assert.IsNotNull(str);
            Assert.AreEqual(value, str, false, CultureInfo.InvariantCulture);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("abc")]
        [DataRow("博麗 霊夢")]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTStringShortened(string value) => TestUtils.Wrap(() =>
        {
            var bytes = TestUtils.CP932Encoding.GetBytes(value);
            Th145AllScoreDataReadObjectHelper(
                TestUtils.MakeByteArray((int)SQOT.String, bytes.Length + 1, bytes), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow("abc")]
        [DataRow("博麗 霊夢")]
        public void Th145AllScoreDataReadObjectTestOTStringExceeded(string value) => TestUtils.Wrap(() =>
        {
            var bytes = TestUtils.CP932Encoding.GetBytes(value).Concat(new byte[1] { 1 }).ToArray();
            var result = Th145AllScoreDataReadObjectHelper(
                TestUtils.MakeByteArray((int)SQOT.String, bytes.Length - 1, bytes), out var obj);
            var str = obj as string;

            Assert.IsTrue(result);
            Assert.IsNotNull(str);
            Assert.AreEqual(value, str, false, CultureInfo.InvariantCulture);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(
            new int[6] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Integer, 456, (int)SQOT.Null },
            new int[1] { 123 },
            new int[1] { 456 },
            DisplayName = "one pair")]
        [DataRow(
            new int[10] {
                (int)SQOT.Table,
                (int)SQOT.Integer, 123, (int)SQOT.Integer, 456,
                (int)SQOT.Integer, 78, (int)SQOT.Integer, 90,
                (int)SQOT.Null },
            new int[2] { 123, 78 },
            new int[2] { 456, 90 },
            DisplayName = "two pairs")]
        public void Th145AllScoreDataReadObjectTestOTTable(int[] array, int[] expectedKeys, int[] expectedValues)
            => TestUtils.Wrap(() =>
            {
                var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);
                var dict = obj as Dictionary<object, object>;

                Assert.IsTrue(result);
                Assert.IsNotNull(dict);
                Assert.AreNotEqual(0, dict.Count);
                CollectionAssert.AreEqual(expectedKeys, dict.Keys.ToArray());
                CollectionAssert.AreEqual(expectedValues, dict.Values.ToArray());
            });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[2] { (int)SQOT.Table, (int)SQOT.Null },
            DisplayName = "empty")]
        public void Th145AllScoreDataReadObjectTestOTTableEmpty(int[] array) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);
            var dict = obj as Dictionary<object, object>;

            Assert.IsTrue(result);
            Assert.IsNotNull(dict);
            Assert.AreEqual(0, dict.Count);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[5] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Integer, (int)SQOT.Null },
            DisplayName = "missing value data")]
        [DataRow(new int[5] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Integer, 456 },
            DisplayName = "missing sentinel")]
        [DataRow(new int[4] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Integer },
            DisplayName = "missing value data and sentinel")]
        [DataRow(new int[4] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing key or value")]
        [DataRow(new int[1] { (int)SQOT.Table },
            DisplayName = "empty and missing sentinel")]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTTableShortened(int[] array) => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[6] { (int)SQOT.Table, 999, 123, (int)SQOT.Integer, 456, (int)SQOT.Null },
            DisplayName = "invalid key type")]
        [DataRow(new int[6] { (int)SQOT.Table, (int)SQOT.Integer, 123, 999, 456, (int)SQOT.Null },
            DisplayName = "invalid value type")]
        [DataRow(new int[6] { (int)SQOT.Table, (int)SQOT.Integer, 123, (int)SQOT.Integer, 456, 999 },
            DisplayName = "invalid sentinel")]
        [DataRow(new int[5] { (int)SQOT.Table, 123, (int)SQOT.Integer, 456, (int)SQOT.Null },
            DisplayName = "missing key type")]
        [DataRow(new int[5] { (int)SQOT.Table, (int)SQOT.Integer, (int)SQOT.Integer, 456, (int)SQOT.Null },
            DisplayName = "missing key data")]
        [DataRow(new int[5] { (int)SQOT.Table, (int)SQOT.Integer, 123, 456, (int)SQOT.Null },
            DisplayName = "missing value type")]
        [DataRow(new int[4] { (int)SQOT.Table, 123, (int)SQOT.Integer, 456 },
            DisplayName = "missing key type and sentinel")]
        [DataRow(new int[4] { (int)SQOT.Table, 123, (int)SQOT.Integer, (int)SQOT.Null },
            DisplayName = "missing key type and value data")]
        [DataRow(new int[4] { (int)SQOT.Table, (int)SQOT.Integer, (int)SQOT.Integer, 456 },
            DisplayName = "missing key data and sentinel")]
        [DataRow(new int[4] { (int)SQOT.Table, (int)SQOT.Integer, 123, 456 },
            DisplayName = "missing value type and sentinel")]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th145AllScoreDataReadObjectTestOTTableInvalid(int[] array) => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(
            new int[7] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            new int[1] { 123 },
            DisplayName = "one element")]
        [DataRow(
            new int[11] {
                (int)SQOT.Array, 2,
                (int)SQOT.Integer, 0, (int)SQOT.Integer, 123,
                (int)SQOT.Integer, 1, (int)SQOT.Integer, 456,
                (int)SQOT.Null },
            new int[2] { 123, 456 },
            DisplayName = "two elements")]
        public void Th145AllScoreDataReadObjectTestOTArray(int[] array, int[] expected) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);
            var resultArray = obj as object[];

            Assert.IsTrue(result);
            Assert.IsNotNull(resultArray);
            Assert.AreNotEqual(0, resultArray.Length);
            CollectionAssert.AreEqual(expected, resultArray);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[3] { (int)SQOT.Array, 0, (int)SQOT.Null },
            DisplayName = "empty")]
        public void Th145AllScoreDataReadObjectTestOTArrayEmpty(int[] array) => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);
            var resultArray = obj as object[];

            Assert.IsTrue(result);
            Assert.IsNotNull(resultArray);
            Assert.AreEqual(0, resultArray.Length);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[7] { (int)SQOT.Array, 999, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "invalid size")]
        [DataRow(new int[6] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Integer, (int)SQOT.Null },
            DisplayName = "missing value data")]
        [DataRow(new int[6] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123 },
            DisplayName = "missing sentinel")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Null },
            DisplayName = "missing value")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Integer },
            DisplayName = "missing value data and sentinel")]
        [DataRow(new int[3] { (int)SQOT.Array, 999, (int)SQOT.Null },
            DisplayName = "empty and invalid number of elements")]
        [DataRow(new int[2] { (int)SQOT.Array, (int)SQOT.Null },
            DisplayName = "empty and missing number of elements")]
        [DataRow(new int[2] { (int)SQOT.Array, 0 },
            DisplayName = "empty and missing sentinel")]
        [DataRow(new int[1] { (int)SQOT.Array },
            DisplayName = "empty and only array type")]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadObjectTestOTArrayShortened(int[] array) => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(new int[7] { (int)SQOT.Array, 0, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "zero size and one element")]
        [DataRow(new int[7] { (int)SQOT.Array, -1, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "negative size")]
        [DataRow(new int[7] { (int)SQOT.Array, 1, 999, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "invalid index type")]
        [DataRow(new int[7] { (int)SQOT.Array, 1, (int)SQOT.Integer, 999, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "invalid index data")]
        [DataRow(new int[7] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, 999, 123, (int)SQOT.Null },
            DisplayName = "invalid value type")]
        [DataRow(new int[7] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, 999 },
            DisplayName = "invalid sentinel")]
        [DataRow(new int[6] { (int)SQOT.Array, 1, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing index type")]
        [DataRow(new int[6] { (int)SQOT.Array, 1, (int)SQOT.Integer, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing index data")]
        [DataRow(new int[6] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, 123, (int)SQOT.Null },
            DisplayName = "missing value type")]
        [DataRow(new int[6] { (int)SQOT.Array, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing number of elements")]
        [DataRow(new int[5] { (int)SQOT.Array, 0, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing number of elements and index type")]
        [DataRow(new int[5] { (int)SQOT.Array, (int)SQOT.Integer, 0, (int)SQOT.Integer, 123 },
            DisplayName = "missing number of elements and sentinel")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, 0, (int)SQOT.Integer, (int)SQOT.Null },
            DisplayName = "missing index type and value data")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, 0, (int)SQOT.Integer, 123 },
            DisplayName = "missing index type and sentinel")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, (int)SQOT.Integer, (int)SQOT.Integer, 123 },
            DisplayName = "missing index data and sentinel")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, (int)SQOT.Integer, 0, 123 },
            DisplayName = "missing value type and sentinel")]
        [DataRow(new int[5] { (int)SQOT.Array, 1, (int)SQOT.Integer, 123, (int)SQOT.Null },
            DisplayName = "missing index")]
        [DataRow(new int[3] { (int)SQOT.Array, 0, 999 },
            DisplayName = "empty and invalid sentinel")]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th145AllScoreDataReadObjectTestOTArrayInvalid(int[] array) => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray(array), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        public void Th145AllScoreDataReadObjectTestOTInstance() => TestUtils.Wrap(() =>
        {
            var result = Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)SQOT.Instance), out var obj);

            Assert.IsTrue(result);
            Assert.IsNotNull(obj);
        });

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [DataTestMethod]
        [DataRow(SQOT.UserData)]
        [DataRow(SQOT.Closure)]
        [DataRow(SQOT.NativeClosure)]
        [DataRow(SQOT.Generator)]
        [DataRow(SQOT.UserPointer)]
        [DataRow(SQOT.Thread)]
        [DataRow(SQOT.FuncProto)]
        [DataRow(SQOT.Class)]
        [DataRow(SQOT.WeakRef)]
        [DataRow(SQOT.Outer)]
        [DataRow((SQOT)999)]
        [ExpectedException(typeof(InvalidDataException))]
        public void Th145AllScoreDataReadObjectTestUnsupported(SQOT type) => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataReadObjectHelper(TestUtils.MakeByteArray((int)type), out var obj);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTest() => TestUtils.Wrap(() =>
        {
            var properties = GetValidProperties();

            var allScoreData = Th145AllScoreDataWrapper.Create(MakeByteArray(properties));

            Validate(allScoreData, properties);
        });

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th145AllScoreDataReadFromTestNull() => TestUtils.Wrap(() =>
        {
            var allScoreData = new Th145AllScoreDataWrapper();
            allScoreData.ReadFrom(null);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th145AllScoreDataReadFromTestEmpty() => TestUtils.Wrap(() =>
        {
            Th145AllScoreDataWrapper.Create(new byte[0]);

            Assert.Fail(TestUtils.Unreachable);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestNoKey() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(TestUtils.MakeByteArray((int)SQOT.Null));

            Assert.AreEqual(default, allScoreData.StoryProgress.Value);
            Assert.IsNull(allScoreData.StoryClearFlags);
            Assert.AreEqual(default, allScoreData.EndingCount.Value);
            Assert.AreEqual(default, allScoreData.Ending2Count.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageTanuki1.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageTanuki2.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledStageKokoro.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledSt27.Value);
            Assert.AreEqual(default, allScoreData.IsEnabledSt28.Value);
            Assert.AreEqual(default, allScoreData.IsPlayableMamizou.Value);
            Assert.AreEqual(default, allScoreData.IsPlayableKokoro.Value);
            Assert.IsNull(allScoreData.BgmFlags);
            Assert.IsNull(allScoreData.ClearRanks);
            Assert.IsNull(allScoreData.ClearTimes);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestNoTables() => TestUtils.Wrap(() =>
        {
            var storyProgressValue = 1;

            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("story_progress", storyProgressValue))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.AreEqual(storyProgressValue, allScoreData.StoryProgress);
            Assert.IsNull(allScoreData.StoryClearFlags);
            Assert.IsNull(allScoreData.BgmFlags);
            Assert.IsNull(allScoreData.ClearRanks);
            Assert.IsNull(allScoreData.ClearTimes);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidStoryClear() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("story_clear", 1))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNull(allScoreData.StoryClearFlags);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidStoryClearValue() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("story_clear", new float[] { 123f }))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNotNull(allScoreData.StoryClearFlags);
            Assert.AreEqual(0, allScoreData.StoryClearFlags.Count);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidEnableBgm() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("enable_bgm", 1))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNull(allScoreData.BgmFlags);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearRank() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_rank", 1))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNull(allScoreData.ClearRanks);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearRankValuePerLevel() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_rank", new float[] { 123f }))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNotNull(allScoreData.ClearRanks);
            Assert.AreEqual(0, allScoreData.ClearRanks.Count);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearRankValuePerChara() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_rank", new float[][] { new float[] { 123f } }))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNotNull(allScoreData.ClearRanks);
            Assert.AreEqual(1, allScoreData.ClearRanks.Count);
            Assert.IsNotNull(allScoreData.ClearRanks.First().Value);
            Assert.AreEqual(0, allScoreData.ClearRanks.First().Value.Count);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearTime() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_time", 1))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNull(allScoreData.ClearTimes);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearTimeValuePerLevel() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_time", new float[] { 123f }))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNotNull(allScoreData.ClearTimes);
            Assert.AreEqual(0, allScoreData.ClearTimes.Count);
        });

        [TestMethod]
        public void Th145AllScoreDataReadFromTestInvalidClearTimeValuePerChara() => TestUtils.Wrap(() =>
        {
            var allScoreData = Th145AllScoreDataWrapper.Create(new byte[0]
                // .Concat(TestUtils.MakeByteArray((int)SQOT.Table)
                .Concat(TestUtils.MakeSQByteArray("clear_time", new float[][] { new float[] { 123f } }))
                .Concat(TestUtils.MakeByteArray((int)SQOT.Null))
                .ToArray());

            Assert.IsNotNull(allScoreData.ClearTimes);
            Assert.AreEqual(1, allScoreData.ClearTimes.Count);
            Assert.IsNotNull(allScoreData.ClearTimes.First().Value);
            Assert.AreEqual(0, allScoreData.ClearTimes.First().Value.Count);
        });
    }
}
