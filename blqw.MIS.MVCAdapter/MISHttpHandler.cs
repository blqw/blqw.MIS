using System;
using System.Web;
using System.Web.Routing;
using blqw.MIS.Descriptor;
using System.Text;

namespace blqw.MIS.MVCAdapter
{
    public partial class MISRouteHandler : IRouteHandler
    {

        internal class MISHttpHandler : IHttpHandler
        {
            /// <summary>
            /// 获取一个值，该值指示其他请求是否可以使用 <see cref="T:System.Web.IHttpHandler" /> 实例。
            /// </summary>
            /// <returns>如果 <see cref="T:System.Web.IHttpHandler" /> 实例可再次使用，则为 true；否则为 false。</returns>
            public bool IsReusable => true;

            public void ProcessRequest(HttpContext httpContext)
            {
                var api = httpContext?.Request?.RequestContext?.RouteData?.DataTokens?["mis.api"] as ApiDescriptor;
                if (api == null)
                {
                    httpContext.Response.StatusCode = 404;
                    httpContext.Response.Write("接口不存在");
                    return;
                }

                byte[] content;
                ApiCallContext context = null;
                try
                {
                    var dataProvider = new DataProvider(httpContext);
                    context = api.Invoke(dataProvider);
                    dataProvider.SaveSession(context.Session);
                    var ex = context.Exception ?? context.Result as Exception;
                    if (ex == null)
                    {
                        content = context.Result == null
                            ? new byte[0]
                            : Encoding.UTF8.GetBytes(Json.ToJsonString(context.Result));
                        httpContext.Response.ContentType = "application/json;charset=utf-8";
                        httpContext.Response.StatusCode = 200;
                    }
                    else if (ex is ApiException)
                    {
                        content = Encoding.UTF8.GetBytes(Json.ToJsonString(new
                        {
                            ErrorCode = ex.HResult,
                            Message = ex.Message,
                            DetailMessage = ex.InnerException?.ToString()
                        }));
                        httpContext.Response.ContentType = "application/json;charset=utf-8";
                        httpContext.Response.StatusCode = 251;
                    }
                    else
                    {
                        content = Encoding.UTF8.GetBytes(context.Exception?.ToString());
                        httpContext.Response.ContentType = "text/plain;charset=utf-8";
                        httpContext.Response.StatusCode = 501;
                    }
                }
                catch (Exception ex)
                {
                    content = Encoding.UTF8.GetBytes(ex.ToString());
                    httpContext.Response.ContentType = "text/plain;charset=utf-8";
                    httpContext.Response.StatusCode = 500;
                }
                httpContext.Response.BinaryWrite(content);
                api.Container.OnEndRequest(context);
            }
        }
    }
}
