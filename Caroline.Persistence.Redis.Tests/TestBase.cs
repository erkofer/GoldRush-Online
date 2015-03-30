using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Caroline.Persistence.Redis.Tests
{
    public abstract class TestBase
    {
        protected static void Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                if (typeof (TException) != ex.GetType())
                    Assert.Fail();
            }
        }
    }
}
