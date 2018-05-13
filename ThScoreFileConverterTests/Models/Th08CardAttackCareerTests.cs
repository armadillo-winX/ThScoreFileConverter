﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace ThScoreFileConverter.Models.Tests
{
    [TestClass()]
    public class Th08CardAttackCareerTests
    {
        [TestMethod()]
        public void Th08CardAttackCareerTest()
        {
            try
            {
                var career = new Th08CardAttackCareerWrapper();

                Assert.AreEqual(0, career.MaxBonuses.Count);
                Assert.AreEqual(0, career.TrialCounts.Count);
                Assert.AreEqual(0, career.ClearCounts.Count);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        public void Th08CardAttackCareerReadFromTest()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(13);
                var trialCounts = TestUtils.MakeRandomArray<int>(13);
                var clearCounts = TestUtils.MakeRandomArray<int>(13);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                CollectionAssert.AreEqual(maxBonuses, career.MaxBonuses.Values.ToArray());
                CollectionAssert.AreEqual(trialCounts, career.TrialCounts.Values.ToArray());
                CollectionAssert.AreEqual(clearCounts, career.ClearCounts.Values.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Th08CardAttackCareerReadFromTestNull()
        {
            try
            {
                var career = new Th08CardAttackCareerWrapper();
                career.ReadFrom(null);
                Assert.Fail(TestUtils.Unreachable);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "career")]
        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08CardAttackCareerReadFromTestShortenedMaxBonuses()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(12);
                var trialCounts = TestUtils.MakeRandomArray<int>(13);
                var clearCounts = TestUtils.MakeRandomArray<int>(13);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                Assert.Fail(TestUtils.Unreachable);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        public void Th08CardAttackCareerReadFromTestExceededMaxBonuses()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(14);
                var trialCounts = TestUtils.MakeRandomArray<int>(13);
                var clearCounts = TestUtils.MakeRandomArray<int>(13);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                CollectionAssert.AreNotEqual(maxBonuses, career.MaxBonuses.Values.ToArray());
                CollectionAssert.AreEqual(maxBonuses.Take(13).ToArray(), career.MaxBonuses.Values.ToArray());
                CollectionAssert.AreNotEqual(trialCounts, career.TrialCounts.Values.ToArray());
                CollectionAssert.AreNotEqual(clearCounts, career.ClearCounts.Values.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "career")]
        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08CardAttackCareerReadFromTestShortenedTrialCounts()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(13);
                var trialCounts = TestUtils.MakeRandomArray<int>(12);
                var clearCounts = TestUtils.MakeRandomArray<int>(13);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                Assert.Fail(TestUtils.Unreachable);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        public void Th08CardAttackCareerReadFromTestExceededTrialCounts()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(13);
                var trialCounts = TestUtils.MakeRandomArray<int>(14);
                var clearCounts = TestUtils.MakeRandomArray<int>(13);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                CollectionAssert.AreEqual(maxBonuses, career.MaxBonuses.Values.ToArray());
                CollectionAssert.AreNotEqual(trialCounts, career.TrialCounts.Values.ToArray());
                CollectionAssert.AreEqual(trialCounts.Take(13).ToArray(), career.TrialCounts.Values.ToArray());
                CollectionAssert.AreNotEqual(clearCounts, career.ClearCounts.Values.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "career")]
        [TestMethod()]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Th08CardAttackCareerReadFromTestShortenedClearCounts()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(13);
                var trialCounts = TestUtils.MakeRandomArray<int>(13);
                var clearCounts = TestUtils.MakeRandomArray<int>(12);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                Assert.Fail(TestUtils.Unreachable);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        [TestMethod()]
        public void Th08CardAttackCareerReadFromTestExceededClearCounts()
        {
            try
            {
                var maxBonuses = TestUtils.MakeRandomArray<uint>(13);
                var trialCounts = TestUtils.MakeRandomArray<int>(13);
                var clearCounts = TestUtils.MakeRandomArray<int>(14);

                var career = Th08CardAttackCareerWrapper.Create(
                    TestUtils.MakeByteArray(maxBonuses, trialCounts, clearCounts));

                CollectionAssert.AreEqual(maxBonuses, career.MaxBonuses.Values.ToArray());
                CollectionAssert.AreEqual(trialCounts, career.TrialCounts.Values.ToArray());
                CollectionAssert.AreNotEqual(clearCounts, career.ClearCounts.Values.ToArray());
                CollectionAssert.AreEqual(clearCounts.Take(13).ToArray(), career.ClearCounts.Values.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
    }
}