using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using blqw.SIF.Descriptor;
using Microsoft.Owin;
using System.Text;
using System.Linq;

namespace blqw.SIF.Owin
{
    public sealed class SIFMiddleware : OwinMiddleware
    {
        private ApiContainer _container;
        private RouteTable _routeTable;

        public SIFMiddleware(OwinMiddleware next)
            : base(next)
        {
            _container = new ApiContainer("Owin", new OwinProvider());
            _routeTable = new RouteTable(_container.ApiCollection);
            Console.WriteLine($"载入接口:{_container.ApiCollection.Apis.Count}个");
        }


        public override async Task Invoke(IOwinContext context)
        {
            var api = _routeTable.Select(context);
            if (api != null)
            {
                var dataProvider = new DataProvider(context);
                var result = api.Invoke(dataProvider);
                var ex = result as Exception;
                byte[] content;
                if (ex == null)
                {
                    content = Encoding.UTF8.GetBytes(Json.ToJsonString(result));
                    context.Response.ContentType = "application/json;charset=utf-8";
                    context.Response.StatusCode = 200;
                }
                else if (ex is ApiException)
                {
                    content = Encoding.UTF8.GetBytes(Json.ToJsonString(new { ErrorCode = ex.HResult, Message = ex.Message, DetailMessage = ex.InnerException?.ToString() }));
                    context.Response.ContentType = "application/json;charset=utf-8";
                    context.Response.StatusCode = 251;
                }
                else
                {
                    content = Encoding.UTF8.GetBytes(result.ToString());
                    context.Response.ContentType = "text/plain;charset=utf-8";
                    context.Response.StatusCode = 500;
                }
                context.Response.ContentLength = content.Length;
                context.Response.Expires = DateTimeOffset.Now;
                await context.Response.WriteAsync(content);
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}