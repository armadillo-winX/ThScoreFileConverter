﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models
{
    [TestClass]
    public class ExceptionOccurredEventArgsTests
    {
        [TestMethod]
        public void ExceptionOccurredEventArgsTest()
        {
            // NOTE: creating an Exception instance causes CA2201.
            var ex = new NotImplementedException();
            var args = new ExceptionOccurredEventArgs(ex);
            Assert.AreSame(ex, args.Exception);
        }

        [TestMethod]
        public void ExceptionOccurredEventArgsTestNull()
            => _ = Assert.ThrowsException<ArgumentNullException>(() => _ = new ExceptionOccurredEventArgs(null!));
    }
}
