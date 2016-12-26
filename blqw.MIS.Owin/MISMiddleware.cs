using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using blqw.MIS.Descriptor;
using Microsoft.Owin;
using System.Text;
using System.Linq;
using blqw.MIS.DataModification;

namespace blqw.MIS.OwinAdapter
{
    public sealed class MISMiddleware : OwinMiddleware
    {
        private ApiContainer _container;
        private RouteTable _routeTable;

        public MISMiddleware(OwinMiddleware next)
            : base(next)
        {
            var owin = new OwinProvider();
            owin.GlobalModifications.Add(new TrimAttribute());

            _container = new ApiContainer(owin);
            _routeTable = new RouteTable(_container.ApiCollection);
            Console.WriteLine($"载入接口:{_container.ApiCollection.Apis.Count}个");
        }

        public override async Task Invoke(IOwinContext owin)
        {
            var api = _routeTable.Select(owin);
            if (api != null)
            {
                byte[] content;
                ApiCallContext context = null;
                try
                {
                    _container.OnBeginRequest();
                    var dataProvider = new DataProvider(owin);
                    context = api.Invoke(dataProvider);
                    dataProvider.SaveSession(context.Session);
                    var ex = context.Exception ?? context.Result as Exception;
                    if (ex == null)
                    {
                        content = context.Result == null ? new byte[0] : Encoding.UTF8.GetBytes(Json.ToJsonString(context.Result));
                        owin.Response.ContentType = "application/json;charset=utf-8";
                        owin.Response.StatusCode = 200;
                    }
                    else if (ex is ApiException)
                    {
                        content = Encoding.UTF8.GetBytes(Json.ToJsonString(new { ErrorCode = ex.HResult, Message = ex.Message, DetailMessage = ex.InnerException?.ToString() }));
                        owin.Response.ContentType = "application/json;charset=utf-8";
                        owin.Response.StatusCode = 251;
                    }
                    else
                    {
                        content = Encoding.UTF8.GetBytes(context.Exception?.ToString());
                        owin.Response.ContentType = "text/plain;charset=utf-8";
                        owin.Response.StatusCode = 501;
                    }
                }
                catch (Exception ex)
                {
                    content = Encoding.UTF8.GetBytes(ex.ToString());
                    owin.Response.ContentType = "text/plain;charset=utf-8";
                    owin.Response.StatusCode = 500;
                }
                owin.Response.ContentLength = content.Length;
                owin.Response.Expires = DateTimeOffset.Now;
                await owin.Response.WriteAsync(content);
                _container.OnEndRequest(context);
            }
            else
            {
                await Next.Invoke(owin);
            }
        }
    }
}