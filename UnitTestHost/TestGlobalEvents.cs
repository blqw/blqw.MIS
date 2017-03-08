using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using blqw.MIS;
using BizDemo;
using blqw;
using blqw.IOC;
using blqw.MIS.UnitTest;
using System.Threading.Tasks;
using System.Text;

namespace UnitTestHost
{
    [TestClass]
    public class TestGlobalEvents
    {
        [TestMethod]
        public void TestMethod1()
        {
            MyClass.Buffer = new StringBuilder();
            UnitTestManager.Call(() => new Test().Hello());
            var txt = MyClass.Buffer.ToString();
            Assert.AreEqual("OnReady,OnApiCreated,OnPropertiesParsed,OnArgumentsParsed,OnEnding,", txt);
        }

    }

    public class MyClass : ApiGlobalEvents
    {
        public static StringBuilder Buffer;
        /// <summary>
        /// 这是第一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnReady(IRequest request)
            => Buffer?.Append(nameof(OnReady) + ",");

        /// <summary>
        /// Api未找到
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnApiNotFound(IRequest request)
            => Buffer?.Append(nameof(OnApiNotFound) + ",");

        /// <summary>
        /// Api创建完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnApiCreated(IRequest request)
            => Buffer?.Append(nameof(OnApiCreated) + ",");

        /// <summary>
        /// Api参数解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnArgumentsParsed(IRequest request)
            => Buffer?.Append(nameof(OnArgumentsParsed) + ",");

        /// <summary>
        /// Api属性解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnPropertiesParsed(IRequest request)
            => Buffer?.Append(nameof(OnPropertiesParsed) + ",");
        
        /// <summary>
        /// 这是最后一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnEnding(IRequest request)
            => Buffer?.Append(nameof(OnEnding) + ",");

        /// <summary>
        /// 出现未处理的非 <seealso cref="ApiException"/> 异常时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnUnprocessException(IRequest request)
            => Buffer?.Append(nameof(OnUnprocessException) + ",");

        /// <summary>
        /// 出现 <see cref="ApiException"/> 时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnApiException(IRequest request)
            => Buffer?.Append(nameof(OnApiException) + ",");

        /// <summary>
        /// 默认情况下出现任何异常都触发,但重新<see cref="OnApiException"/> 或 <see cref="OnUnprocessException"/> 后不,对应的异常不会触发该事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnException(IRequest request)
            => Buffer?.Append(nameof(OnException) + ",");

        /// <summary>
        /// 准备返回数据时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public override async Task OnBeginResponse(IRequest request)
            => Buffer?.Append(nameof(OnBeginResponse) + ",");
    }
}
