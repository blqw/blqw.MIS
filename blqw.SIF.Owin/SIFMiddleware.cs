using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using blqw.SIF.Descriptor;
using Microsoft.Owin;

namespace blqw.SIF.Owin
{
    public sealed class SIFMiddleware : OwinMiddleware
    {
        private Container _container;
        private RouteTable _routeTable;

        public SIFMiddleware(OwinMiddleware next)
            : base(next)
        {
            _container = new Container("Owin", new OwinProvider());
            _routeTable = new RouteTable(_container.Apis);
        }


        public override async Task Invoke(IOwinContext context)
        {
            var api = _routeTable.Select(context);
            if (api != null)
            {
                var dataGetter = new DataGetter(context);
                var obj = api.CreateInstance(dataGetter);
                var result = api.Invoke(obj, dataGetter);

                var content = result.ToString();
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = content.Length;
                context.Response.StatusCode = 200;
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