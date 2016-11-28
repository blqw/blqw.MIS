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

        public SIFMiddleware(OwinMiddleware next)
            : base(next)
        {
            _container = new Container("Owin", new OwinProvider());
            _routes = new Dictionary<string, ApiDescriptor>();
        }

        private readonly Dictionary<string, ApiDescriptor> _routes;
        public override Task Invoke(IOwinContext context)
        {
            ApiDescriptor api;
            if (_routes.TryGetValue(context.Request.Path.Value, out api))
            {
                var param = new Dictionary<string, object>();
                foreach (var q in context.Request.Query)
                {
                    var q -
                }
                var obj = Activator.CreateInstance(api.Type);
                api.Invoke(obj, null);
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = content.Length;
                context.Response.StatusCode = 200;
                context.Response.Expires = DateTimeOffset.Now;
            }

            return Next.Invoke(context);
        }
    }
}