﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter.Squirrel;
using ThScoreFileConverterTests.Models;

namespace ThScoreFileConverterTests.Squirrel
{
    [TestClass]
    public class SQNullTests
    {
        [TestMethod]
        public void InstanceTest()
        {
            var sqnull = SQNull.Instance;

            Assert.AreEqual(SQObjectType.Null, sqnull.Type);
        }

        internal static SQNull CreateTestHelper(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);

            return SQNull.Create(reader);
        }

        [TestMethod]
        public void CreateTest()
        {
            var sqnull = CreateTestHelper(TestUtils.MakeByteArray((int)SQObjectType.Null));

            Assert.AreEqual(SQObjectType.Null, sqnull.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateTestNull()
        {
            _ = SQNull.Create(null!);

            Assert.Fail(TestUtils.Unreachable);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void CreateTestInvalid()
        {
            _ = CreateTestHelper(TestUtils.MakeByteArray((int)SQObjectType.Bool));

            Assert.Fail(TestUtils.Unreachable);
        }

        [TestMethod]
        public void EqualsTestNull() => Assert.IsFalse(SQNull.Instance.Equals(null!));

        [TestMethod]
        public void EqualsTestNullObject() => Assert.IsFalse(SQNull.Instance.Equals((object)null!));

        [TestMethod]
        public void EqualsTestInvalidType() => Assert.IsFalse(SQNull.Instance.Equals(SQBool.True));

        [TestMethod]
        public void EqualsTestSelf() => Assert.IsTrue(SQNull.Instance.Equals(SQNull.Instance));

        [TestMethod]
        public void EqualsTestSelfObject() => Assert.IsTrue(SQNull.Instance.Equals(SQNull.Instance as object));

        [TestMethod]
        public void EqualsTest()
        {
            var created = CreateTestHelper(TestUtils.MakeByteArray((int)SQObjectType.Null));

            Assert.IsTrue(SQNull.Instance.Equals(created));
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            var created = CreateTestHelper(TestUtils.MakeByteArray((int)SQObjectType.Null));

            Assert.AreEqual(SQNull.Instance.GetHashCode(), created.GetHashCode());
        }
    }
}
