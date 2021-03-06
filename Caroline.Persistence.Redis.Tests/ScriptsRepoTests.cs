﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Redis.Tests
{
    [TestClass]
    public class ScriptsRepoTests : TestBase
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var expected = new Dictionary<string, string>
            {
                {"EmbeddedResource.Resource", "RESOURCERESOURCERESOURCE"},
                {"Resources.NestedEmbeddedResource.Resource", "Resource"}
            };

            var result = new ReadOnlyTypeSafeDictionaryMock(expected);

            Assert.AreEqual("RESOURCERESOURCERESOURCE", result.EmbeddedResource);
            Assert.AreEqual("Resource", result.NestedEmbeddedResource);
        }

        class ReadOnlyTypeSafeDictionaryMock : ReadOnlyTypeSafeDictionary<string>
        {
            public ReadOnlyTypeSafeDictionaryMock(IReadOnlyDictionary<string, string> scripts)
                : base(scripts, '.')
            {

            }

            public string EmbeddedResource { get; set; }
            public string NestedEmbeddedResource { get; set; }
        }
    }
}
