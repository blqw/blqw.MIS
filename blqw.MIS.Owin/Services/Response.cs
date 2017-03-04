using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace blqw.MIS.Owin.Services
{
    public class Response : IResponse
    {
        public Response(Request request)
        {
            Request = request;
            Exception = request.Result as Exception;
            IsError = Exception != null;
        }

        /// <summary>
        /// 获取真实响应对象
        /// </summary>
        /// <returns></returns>
        public object GetActualResponse() => Request.Result;

        /// <summary>
        /// 请求
        /// </summary>
        public Request Request { get; }

        IRequest IResponse.Request => Request;

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetData() => Request.Result == null ? null : Encoding.UTF8.GetBytes(Request.Result.ToString());

        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        public string ToString(string format)
            => ToString(format, null);

        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool IsError { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; }

        public string ToString(string format, IFormatProvider formatProvider)
            => GetActualResponse()?.ToString() ?? "<null>";

        public override string ToString() => ToString(null, null);
    }
}
