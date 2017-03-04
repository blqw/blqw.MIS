using System;
using System.Web;
using System.Web.Routing;
using System.Text;
using blqw.MIS.Descriptors;
using blqw.MIS.MVC.Services;

namespace blqw.MIS.MVC
{
    internal class HttpRouteHandler : IHttpHandler, IRouteHandler
    {
        private Scheduler _scheduler;

        public HttpRouteHandler(string urlTemplate)
        {
            var container = new ApiContainer("Mvc5", ExportedTypes.Enumerable());
            var entry = new ServiceEntry(container, urlTemplate);
            _scheduler = new Scheduler(entry);
        }

        /// <summary>
        /// 获取一个值，该值指示其他请求是否可以使用 <see cref="T:System.Web.IHttpHandler" /> 实例。
        /// </summary>
        /// <returns>如果 <see cref="T:System.Web.IHttpHandler" /> 实例可再次使用，则为 true；否则为 false。</returns>
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext httpContext)
        {
            byte[] content = null;
            try
            {
                var request = new Request(httpContext);
                var response = _scheduler.Execute(new RequestSetter(request));
                var httpResponse = httpContext.Response;
                if (response == null)
                {
                    httpResponse.StatusCode = 404;
                    httpResponse.ContentType = "text/plain;charset=utf-8";
                    content = Encoding.UTF8.GetBytes("接口不存在");
                }
                else if (response.IsError)
                {
                    var ex = response.Exception.ProcessException();
                    if (ex is ApiNoResultException)
                    {
                        httpResponse.StatusCode = 205;
                    }
                    else if (ex is ApiRequestException e)
                    {
                        httpResponse.StatusCode = 400;
                        if (e.Detail != null)
                        {
                            httpResponse.Headers["MIS-ErrorDetail"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(e.Detail));
                        }
                    }
                    else
                    {
                        httpResponse.StatusCode = 500;
                    }
                    httpResponse.ContentType = "text/plain;charset=utf-8";
                    httpResponse.Headers["MIS-ErrorMessage"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.Message));
                    httpResponse.Headers["MIS-ErrorStackTrace"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.ToString()));
                    content = Encoding.UTF8.GetBytes("请求中出现错误");
                }
                else
                {
                    httpResponse.ContentType = "text/json;charset=utf-8";
                    httpResponse.StatusCode = 200;
                    if (request.Result != null)
                    {
                        content = Encoding.UTF8.GetBytes(response.GetActualResponse().ToJsonString());
                    }
                }
            }
            catch (Exception e)
            {
                content = Encoding.UTF8.GetBytes(e.ToString());
                httpContext.Response.ContentType = "text/plain;charset=utf-8";
                httpContext.Response.StatusCode = 500;
            }

            if (content != null)
            {
                httpContext.Response.BinaryWrite(content);
            }
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => this;
    }
}
