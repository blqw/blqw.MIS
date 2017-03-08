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
        /// ���ǵ�һ�����������¼�
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnReady(IRequest request)
            => Buffer?.Append(nameof(OnReady) + ",");

        /// <summary>
        /// Apiδ�ҵ�
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnApiNotFound(IRequest request)
            => Buffer?.Append(nameof(OnApiNotFound) + ",");

        /// <summary>
        /// Api�������
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnApiCreated(IRequest request)
            => Buffer?.Append(nameof(OnApiCreated) + ",");

        /// <summary>
        /// Api�����������
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnArgumentsParsed(IRequest request)
            => Buffer?.Append(nameof(OnArgumentsParsed) + ",");

        /// <summary>
        /// Api���Խ������
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnPropertiesParsed(IRequest request)
            => Buffer?.Append(nameof(OnPropertiesParsed) + ",");
        
        /// <summary>
        /// �������һ�����������¼�
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnEnding(IRequest request)
            => Buffer?.Append(nameof(OnEnding) + ",");

        /// <summary>
        /// ����δ����ķ� <seealso cref="ApiException"/> �쳣ʱ����
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnUnprocessException(IRequest request)
            => Buffer?.Append(nameof(OnUnprocessException) + ",");

        /// <summary>
        /// ���� <see cref="ApiException"/> ʱ����
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnApiException(IRequest request)
            => Buffer?.Append(nameof(OnApiException) + ",");

        /// <summary>
        /// Ĭ������³����κ��쳣������,������<see cref="OnApiException"/> �� <see cref="OnUnprocessException"/> ��,��Ӧ���쳣���ᴥ�����¼�
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnException(IRequest request)
            => Buffer?.Append(nameof(OnException) + ",");

        /// <summary>
        /// ׼����������ʱ����
        /// </summary>
        /// <param name="request">��ǰ����</param>
        public override async Task OnBeginResponse(IRequest request)
            => Buffer?.Append(nameof(OnBeginResponse) + ",");
    }
}
