﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter.Models.Th17;
using ThScoreFileConverterTests.Models.Th095;

namespace ThScoreFileConverterTests.Models.Th17
{
    [TestClass]
    public class HeaderTests
    {
        [TestMethod]
        public void IsValidTest()
        {
            var array = HeaderBaseTests.MakeByteArray(HeaderBaseTests.MakeProperties("TH71"));
            var header = TestUtils.Create<Header>(array);

            Assert.IsTrue(header.IsValid);
        }

        [TestMethod]
        public void IsValidTestInvalidSignature()
        {
            var array = HeaderBaseTests.MakeByteArray(HeaderBaseTests.MakeProperties("th71"));
            var header = TestUtils.Create<Header>(array);

            Assert.IsFalse(header.IsValid);
        }

        [TestMethod]
        public void IsValidTestExceededSignature()
        {
            var array = HeaderBaseTests.MakeByteArray(HeaderBaseTests.MakeProperties("TH71."));
            var header = TestUtils.Create<Header>(array);

            Assert.IsFalse(header.IsValid);
        }
    }
}