using System.Collections.Generic;
using System.Linq;
using Caroline.Persistence.Redis.Tests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Redis.Tests
{
    [TestClass]
    public class EmbeddedResourcesDictionaryTests
    {
        [TestMethod]
        public void AssemblyTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"Caroline.Persistence.Redis.Tests.EmbeddedResource.Resource", "RESOURCERESOURCERESOURCE"},
                {"Caroline.Persistence.Redis.Tests.Resources.NestedEmbeddedResource.Resource", "RESOURCERESOURCERESOURCERESOURCE"}
            };

            var result = new EmbeddedResourcesDictionary(GetType().Assembly);

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void NamespaceTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"EmbeddedResource.Resource", "RESOURCERESOURCERESOURCE"},
                {"Resources.NestedEmbeddedResource.Resource", "RESOURCERESOURCERESOURCERESOURCE"}
            };

            var result = new EmbeddedResourcesDictionary(GetType());

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void NamespaceScopedTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"NestedEmbeddedResource.Resource", "RESOURCERESOURCERESOURCERESOURCE"}
            };

            var result = new EmbeddedResourcesDictionary(typeof(NestedResourcesScope));

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void NamespaceScopedAssemblyTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"NestedEmbeddedResource.Resource", "RESOURCERESOURCERESOURCERESOURCE"}
            };

            var result = new EmbeddedResourcesDictionary(GetType().Assembly, "Caroline.Persistence.Redis.Tests.Resources");

            Assert.IsTrue(expected.SequenceEqual(result));
        }
    }
}
