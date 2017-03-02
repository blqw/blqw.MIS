using blqw.MIS.Owin.Services;
using Microsoft.Owin;
using System;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Owin
{
    public sealed class MISMiddleware : OwinMiddleware
    {
        private readonly Scheduler _scheduler;

        public MISMiddleware(OwinMiddleware next)
            : base(next)
        {
            var container = new ApiContainer("Owin", ExportedTypes.Enumerable());
            _scheduler = new Scheduler(new ServiceEntry(container));
            Console.WriteLine($"载入接口完成,共 {container.Apis.Count} 个");
        }


        public override async Task Invoke(IOwinContext owin)
        {
            var request = new Request(owin);
            try
            {
                var response = await _scheduler.OnExecuteAsync(new RequestSetter(request));
                if (response == null)
                {
                    await Next.Invoke(owin);
                }
            }
            catch (ApiRequestException ex)
            {
                owin.Response.ContentType = "text/json;charset=utf-8";
                owin.Response.StatusCode = ex.ResponseStatusCode;
                owin.Response.Expires = DateTimeOffset.Now;
                var content = Encoding.UTF8.GetBytes(new { ex.Message, Code = ex.HResult }.ToJsonString());
                owin.Response.ContentLength = content.Length;
                await owin.Response.WriteAsync(content);
                request.Result = ex;
                _scheduler.OnError(request, ex);
            }
            catch (Exception ex)
            {
                ex = ex.ProcessException();
                owin.Response.ContentType = "text/plain;charset=utf-8";
                owin.Response.StatusCode = 500;
                owin.Response.Expires = DateTimeOffset.Now;
                var content = Encoding.UTF8.GetBytes(ex.ToString());
                owin.Response.ContentLength = content.Length;
                await owin.Response.WriteAsync(content);
                request.Result = ex;
                _scheduler.OnError(request, ex);
            }
            finally
            {
                _scheduler.OnEnd(request);
            }
        }
    }
}