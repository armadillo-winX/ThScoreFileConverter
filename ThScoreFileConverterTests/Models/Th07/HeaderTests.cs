﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ThScoreFileConverter.Models.Th07;
using Chapter = ThScoreFileConverter.Models.Th06.Chapter;
using ChapterWrapper = ThScoreFileConverterTests.Models.Th06.Wrappers.ChapterWrapper;

namespace ThScoreFileConverterTests.Models.Th07
{
    [TestClass]
    public class HeaderTests
    {
        internal struct Properties
        {
            public string signature;
            public short size1;
            public short size2;
            public byte[] data;
        };

        internal static Properties ValidProperties => new Properties()
        {
            signature = "TH7K",
            size1 = 12,
            size2 = 12,
            data = new byte[] { 0x10, 0x00, 0x00, 0x00 }
        };

        internal static byte[] MakeByteArray(in Properties properties)
            => TestUtils.MakeByteArray(
                properties.signature.ToCharArray(), properties.size1, properties.size2, properties.data);

        internal static void Validate(in Header header, in Properties properties)
        {
            Assert.AreEqual(properties.signature, header.Signature);
            Assert.AreEqual(properties.size1, header.Size1);
            Assert.AreEqual(properties.size2, header.Size2);
            Assert.AreEqual(properties.data[0], header.FirstByteOfData);
        }

        [TestMethod]
        public void HeaderTest() => TestUtils.Wrap(() =>
        {
            var properties = ValidProperties;

            var chapter = ChapterWrapper.Create(MakeByteArray(properties));
            var header = new Header(chapter.Target as Chapter);

            Validate(header, properties);
        });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "header")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HeaderTestNull() => TestUtils.Wrap(() =>
        {
            var header = new Header(null);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "header")]
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void HeaderTestInvalidSignature() => TestUtils.Wrap(() =>
        {
            var properties = ValidProperties;
            properties.signature = properties.signature.ToLowerInvariant();

            var chapter = ChapterWrapper.Create(MakeByteArray(properties));
            var header = new Header(chapter.Target as Chapter);

            Assert.Fail(TestUtils.Unreachable);
        });

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "header")]
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void HeaderTestInvalidSize1() => TestUtils.Wrap(() =>
        {
            var properties = ValidProperties;
            ++properties.size1;
            properties.data = properties.data.Concat(new byte[] { default }).ToArray();

            var chapter = ChapterWrapper.Create(MakeByteArray(properties));
            var header = new Header(chapter.Target as Chapter);

            Assert.Fail(TestUtils.Unreachable);
        });
    }
}