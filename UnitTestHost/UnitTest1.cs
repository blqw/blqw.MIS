using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blqw.UIF;
using BizDemo;
using blqw;

namespace UnitTestHost
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var r = UnitTest.Call(() => new User() { X = 100 }.Get(1, "blqw", 1));
            if (r.Exception != null)
            {
                throw r.Exception;
            }
            Console.WriteLine(r.Result.ToJsonString());
        }
    }
}
