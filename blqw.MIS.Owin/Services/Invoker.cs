using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Services;

namespace blqw.MIS.Owin.Services
{
    class Invoker : DefaultInvoker
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static Invoker Instance { get; } = new Invoker();

        /// <summary>
        /// 执行调用器得到返回值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override object Execute(IRequestBase request)
        {
            var result = request.Result ?? base.Execute(request);
            var req = request.CastRequest();
            req.Result = result;
            return GetResponseAsync(req);
        }


        async Task GetResponseAsync(Request request)
        {
            var response = request.OwinContext.Response;
            response.ContentType = "text/json;charset=utf-8";
            response.Expires = DateTimeOffset.Now;
            if (request.Result is Exception ex)
            {
                await response.WriteErrorAsync(ex);
            }
            if (request.Result != null)
            {
                response.StatusCode = 200;
                response.ReasonPhrase = "OK";
                var content = request.Result.ToJsonString();
                response.ContentLength = content.Length;
                await response.WriteAsync(content);
            }
            else
            {
                response.StatusCode = 205;
                response.ReasonPhrase = "Reset Content";
            }
        }

    }
}
