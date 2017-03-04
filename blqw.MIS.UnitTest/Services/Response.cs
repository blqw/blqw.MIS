using System;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.UnitTest
{
    public class Response : IResponse
    {
        public Response(Request request)
        {
            Request = request;
            Exception = request.Result as Exception;
            IsError = Exception != null;
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => GetActualResponse()?.ToString() ?? "<null>";

        /// <summary>
        /// 获取真实响应对象
        /// </summary>
        /// <returns></returns>
        public object GetActualResponse() => Request.Result;

        /// <summary>
        /// 请求
        /// </summary>
        public Request Request { get; }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetData() => null;

        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        public string ToString(string format)
            => GetActualResponse()?.ToString() ?? "<null>";

        public override string ToString()
            => GetActualResponse()?.ToString() ?? "<null>";

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; }

        IRequest IResponse.Request => Request;
    }
}
