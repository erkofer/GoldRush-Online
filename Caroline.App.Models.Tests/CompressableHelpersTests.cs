using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.App.Models.Tests
{
    [TestClass]
    public class CompressableHelpersTests
    {
        [TestMethod]
        public void VerifyAscendingIdsTest()
        {
            var invalidList = new List<CompressableFake>
            {
                new CompressableFake(50, 20),
                new CompressableFake(21, 30),
                new CompressableFake(52, 35)
            };

            var validList = new List<CompressableFake>
            {
                new CompressableFake(21, 30),
                new CompressableFake(50, 20),
                new CompressableFake(52, 35)
            };

            Assert.IsFalse(CompressableHelpers.VerifyListAscendingIds(invalidList));
            Assert.IsTrue(CompressableHelpers.VerifyListAscendingIds(validList));
        }

        [TestMethod]
        public void CompressListTest()
        {
            var current = new List<CompressableFake>
            {
                new CompressableFake(10, 50),
                new CompressableFake(20, 55),
                new CompressableFake(30, 40),
                new CompressableFake(50, 60),
                new CompressableFake(60, 20),
                new CompressableFake(70, 0),
                new CompressableFake(80, 0),
            };

            var old = new List<CompressableFake>
            {
                new CompressableFake(12, 20),
                new CompressableFake(20, 60),
                new CompressableFake(50, 30),
                new CompressableFake(55, 10),
                new CompressableFake(60, 20),
                new CompressableFake(70, 20),
                new CompressableFake(80, 0)
            };

            var expected = new List<CompressableFake>
            {
                new CompressableFake(10, 50),
                new CompressableFake(20, 55),
                new CompressableFake(30, 40),
                new CompressableFake(50, 60),
                new CompressableFake(70, 0)
            };

            var outList = new List<CompressableFake>();

            // act
            CompressableHelpers.CompressList(current, old, outList);

            Assert.IsTrue(expected.SequenceEqual(outList));
        }
    }
}
