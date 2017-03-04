using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw;
using blqw.MIS;
using blqw.MIS.UnitTest;

namespace BizDemo.Tests
{
    [TestClass()]
    public class TestTests
    {
        [TestMethod()]
        public void HelloTest()
        {
            var r = UnitTestManager.Call(() => new Test().Hello());
            if (r.Exception != null)
            {
                throw r.Exception;
            }
            Assert.AreEqual("Hello", r.Result);
        }

        [TestMethod()]
        public void HelloAsyncTest()
        {
            var r = UnitTestManager.Call(() => new Test().HelloAsync());
            if (r.Exception != null)
            {
                throw r.Exception;
            }
            Assert.AreEqual("Hello", r.Result);
        }

        [TestMethod()]
        public void CheckPropTest()
        {
            var r = UnitTestManager.Call(() => new Test() { Name = "abc" }.CheckProp("abc"));
            if (r.Exception != null)
            {
                throw r.Exception;
            }

            var r2 = UnitTestManager.Call(() => new Test() { Name = "abc1" }.CheckProp("abc"));
            if (r2.Exception == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void CheckPropAsyncTest()
        {
            var r = UnitTestManager.Call(() => new Test() { Name = "abc" }.CheckPropAsync("abc"));
            if (r.Exception != null)
            {
                throw r.Exception;
            }

            var r2 = UnitTestManager.Call(() => new Test() { Name = "abc1" }.CheckPropAsync("abc"));
            if (r2.Exception == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void NotApiTest()
        {
            var r = UnitTestManager.Call(() => new Test().NotApi());
            Assert.IsInstanceOfType(r.Exception,typeof(ApiNotFoundException));
        }
    }
}