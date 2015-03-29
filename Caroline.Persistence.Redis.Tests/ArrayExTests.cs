using System.Linq;
using Caroline.Persistence.Redis.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Redis.Tests
{
    [TestClass]
    public class ArrayExTests
    {
        [TestMethod]
        public void CombineTest()
        {
            var head = new byte[] { 255, 123, 221, 51 };
            var tail = new byte[] { 200, 123, 51, 55 };
            var expected = new byte[] { 255, 123, 221, 51, 200, 123, 51, 55 };

            var result = ArrayEx.Combine(head, tail);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void CombineParamsTest()
        {
            var head = new byte[] { 255, 123, 221, 51 };
            var body = new byte[] { 231, 103, 20, 90 };
            var tail = new byte[] { 200, 123, 51, 55 };
            var expected = new byte[] { 255, 123, 221, 51, 231, 103, 20, 90, 200, 123, 51, 55 };

            var result = ArrayEx.Combine(head, body, tail);

            Assert.IsTrue(expected.SequenceEqual(result));
        }
    }
}
