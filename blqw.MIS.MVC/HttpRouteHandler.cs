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
                var result = _scheduler.Execute(new RequestSetter(request));
                var response = httpContext.Response;
                if (result is Exception ex)
                {
                    ex = ex.ProcessException();
                    if (result is ApiNotFoundException)
                    {
                        response.StatusCode = 404;
                        response.ContentType = "text/plain;charset=utf-8";
                        content = Encoding.UTF8.GetBytes("api不存在");
                    }
                    else if (ex is ApiNoResultException)
                    {
                        response.StatusCode = 205;
                    }
                    else if (ex is ApiRequestException e)
                    {
                        response.StatusCode = 400;
                        content = Encoding.UTF8.GetBytes("请求中出现错误");
                        if (e.Detail != null)
                        {
                            response.Headers["MIS-ErrorDetail"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(e.Detail));
                        }
                    }
                    else
                    {
                        response.StatusCode = 500;
                        content = Encoding.UTF8.GetBytes("服务器异常");
                    }
                    response.ContentType = "text/plain;charset=utf-8";
                    response.Headers["MIS-ErrorMessage"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.Message));
                    response.Headers["MIS-ErrorStackTrace"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.ToString()));
                }
                else
                {
                    response.ContentType = "text/json;charset=utf-8";
                    response.StatusCode = 200;
                    if (result != null)
                    {
                        content = Encoding.UTF8.GetBytes(result.ToJsonString());
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
