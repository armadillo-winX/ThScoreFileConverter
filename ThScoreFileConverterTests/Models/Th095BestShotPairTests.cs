﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter.Models.Th095;
using ThScoreFileConverterTests.Models.Th095;
using ThScoreFileConverterTests.Models.Wrappers;

namespace ThScoreFileConverterTests.Models
{
    [TestClass]
    public class Th095BestShotPairTests
    {
        internal struct Properties
        {
            public string path;
            public IBestShotHeader header;
        };

        internal static Properties GetValidProperties() => new Properties()
        {
            path = @"D:\path\to\東方文花帖\bestshot\bs_09_6.dat",
            header = BestShotHeaderTests.ValidStub
        };

        internal static void Validate(in Th095BestShotPairWrapper pair, in Properties properties)
        {
            Assert.AreEqual(properties.path, pair.Path);
            BestShotHeaderTests.Validate(properties.header, pair.Header);
        }

        [TestMethod]
        public void Th095BestShotPairTest() => TestUtils.Wrap(() =>
        {
            var properties = GetValidProperties();

            var header = TestUtils.Create<BestShotHeader>(BestShotHeaderTests.MakeByteArray(properties.header));
            var pair = new Th095BestShotPairWrapper(properties.path, header);

            Validate(pair, properties);
        });

        [TestMethod]
        public void DeconstructTest() => TestUtils.Wrap(() =>
        {
            var properties = GetValidProperties();

            var header = TestUtils.Create<BestShotHeader>(BestShotHeaderTests.MakeByteArray(properties.header));
            var pair = new Th095BestShotPairWrapper(properties.path, header);
            var (actualPath, actualHeader) = pair;

            Assert.AreEqual(properties.path, actualPath);
            BestShotHeaderTests.Validate(properties.header, actualHeader);
        });
    }
}
