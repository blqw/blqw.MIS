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
        private readonly Task _task;

        public Response(Request request)
        {
            Request = request;
            _task = GetActualResponseAsyncImpl();
        }

        async Task GetActualResponseAsyncImpl()
        {
            var response = Request.OwinContext.Response;
            response.ContentType = "text/json;charset=utf-8";
            response.Expires = DateTimeOffset.Now;
            if (Request.Result != null)
            {
                response.StatusCode = 200;
                response.ReasonPhrase = "OK";
                var content = Request.Result.ToJsonString();
                response.ContentLength = content.Length;
                await response.WriteAsync(content);
            }
            else
            {
                response.StatusCode = 205;
                response.ReasonPhrase = "Reset Content";
            }
        }

        /// <summary>
        /// 获取真实响应对象
        /// </summary>
        /// <returns></returns>
        public async Task<IOwinResponse> GetActualResponseAsync()
        {
            if (_task.IsCompleted == false)
            {
                await _task;
            }
            return Request.OwinContext.Response;
        }

        /// <summary>
        /// 请求
        /// </summary>
        public Request Request { get; }

        IRequest IResponse.Request => Request;

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetDataAsync()
        {
            var response = await GetActualResponseAsync();
            return response.Body.ReadAll();
        }

        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        public string ToString(string format)
            => ToString(format, null);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var request = Request.ActualRequest;
            var response = GetActualResponseAsync().Result;
            switch (format)
            {
                case "all":
                    var buffer = StringBuilderPool.GetOut();
                    buffer.AppendLine(request.Accept);
                    buffer.Append(" ");
                    buffer.Append(response.StatusCode);
                    buffer.Append(" ");
                    buffer.Append(response.ReasonPhrase);
                    foreach (var header in response.Headers)
                    {
                        foreach (var value in header.Value)
                        {
                            buffer.Append(header.Key);
                            buffer.Append(":");
                            buffer.AppendLine(value);
                        }
                    }
                    buffer.AppendLine();
                    buffer.Append((request.ContentType.GetEncoding() ?? Encoding.Default).GetString(response.Body.ReadAll()));
                    return StringBuilderPool.Return(buffer);
                case "body":
                case null:
                default:
                    return (request.ContentType.GetEncoding() ?? Encoding.Default).GetString(response.Body.ReadAll());
            }
        }

        public override string ToString() => ToString(null, null);

        async Task<object> IResponse.GetActualResponseAsync() => await GetActualResponseAsync();
    }
}
