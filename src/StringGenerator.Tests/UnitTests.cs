using NUnit.Framework;
using System;
using System.Linq;

namespace StringGenerator.Tests {
    public class UnitTests {
        [SetUp]
        public void Setup() {
        }

        [Test]
        public void GivenNoParameters_WhenNext_DefaultRandomStringReturned() {
            // arrange
            var g = new CryptoStringGenerator();


            // act
            var s = g.Next();

            // assert
            Assert.That(s.Length, Is.EqualTo(32), "Default random string length should be 32");

        }

        [Test]
        public void GivenInvalidLength_WhenNext_ExceptionIsThrown() {
            // arrange
            var g = new CryptoStringGenerator();


            // act


            // assert
            Assert.Throws<ArgumentException>(() => g.Next(-1, true));

        }

        [Test]
        public void GivenBatchSize_WhenNextBatch_BatchReturnedIsCorrectSize() {
            // arrange
            var g = new CryptoStringGenerator();
            var size = 10;


            // act
            var s = g.NextBatch(size).ToList();

            // assert
            Assert.That(s.Count, Is.EqualTo(size), $"Batch size should be {size}");
            Assert.That(s[0].Length, Is.EqualTo(32), "Default random string length should be 32");
        }
    }
}