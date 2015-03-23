using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Models.Tests
{
    [TestClass]
    public class IpAddressTests
    {
        [TestMethod]
        public void SerializeTest()
        {
            var expected = "251:1312:3145414:13";

            var val = new IpAddress(251, 1312, 3145414, 13);
            var actual = IpAddress.Serialize(val);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            var expected = new IpAddress(983274, 3131, 131311222, 313);

            const string serial = "983274:3131:131311222:313";

            var result = IpAddress.Deserialize(serial);

            Assert.AreEqual(expected,result);
        }
    }
}
