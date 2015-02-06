using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caroline.Persistence.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Tests
{
    [TestClass]
    public class TypeExTests : TestBase
    {
        [TestMethod]
        public void GetBaseClassesPropertiesTest()
        {
            var derivedType = typeof (Derived);
            var allExpected = new List<PropertyInfo>
            {
                derivedType.GetProperty("DerivedProperty"),
                derivedType.GetProperty("DerivedString"),
                derivedType.GetProperty("MiddleProperty"),
                derivedType.GetProperty("MiddleByteProperty"),
                derivedType.GetProperty("BaseProperty")
            };
            var filterExpected = new List<PropertyInfo>
            {
                derivedType.GetProperty("DerivedProperty"),
                derivedType.GetProperty("MiddleProperty"),
                derivedType.GetProperty("BaseProperty")
            };
            
            Throws<ArgumentException>(() => derivedType.GetProperties(typeof(int)));

            var allResult = TypeEx.GetProperties(derivedType);
            var filterResult = derivedType.GetProperties(typeof (int));
            
            Assert.IsTrue(allResult.SequenceEqual(allExpected));
            Assert.IsTrue(filterResult.SequenceEqual(filterExpected));
        }

        class Base
        {
            public int BaseProperty { get; set; }
        }

        class Middle : Base
        {
            public int MiddleProperty { get; set; }
            public byte[] MiddleByteProperty { get; set; }
        }

        class Derived : Middle
        {
            public int DerivedProperty { get; set; }
            public string DerivedString { get; set; }
            private byte[] PrivateProperty { get; set; }
        }
    }
}
