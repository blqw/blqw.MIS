using blqw.MIS.Owin.Services;
using Microsoft.Owin;
using System;
using System.Linq;
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
            var entry = new ServiceEntry(container);
            _scheduler = new Scheduler(entry);
            var urls = ((Selector)entry.Selector).GetAllUrls().ToArray();
            foreach (var url in urls)
            {
                Console.WriteLine(url);
            }
            Console.WriteLine($"载入接口完成,共 {urls.Length} 个");
        }


        public override async Task Invoke(IOwinContext owin)
        {
            byte[] content = null;
            try
            {
                var request = new Request(owin);
                var response = await _scheduler.ExecuteAsync(new RequestSetter(request));
                var oresponse = request.OwinContext.Response;
                oresponse.Expires = DateTimeOffset.Now;
                oresponse.Headers["MIS-RequestId"] = request.Id;
                if (response == null)
                {
                    await Next.Invoke(owin);
                    if (oresponse.StatusCode == 404)
                    {
                        oresponse.ContentType = "text/plain;charset=utf-8";
                        content = Encoding.UTF8.GetBytes("api不存在");
                    }
                }
                else if (response.IsError)
                {
                    var ex = response.Exception.ProcessException();
                    if (ex is ApiNoResultException)
                    {
                        oresponse.StatusCode = 205;
                    }
                    else if (ex is ApiRequestException e)
                    {
                        oresponse.StatusCode = 400;
                        if (e.Detail != null)
                        {
                            oresponse.Headers["MIS-ErrorDetail"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(e.Detail));
                        }
                    }
                    else
                    {
                        oresponse.StatusCode = 500;
                    }
                    oresponse.ContentType = "text/plain;charset=utf-8";
                    oresponse.Headers["MIS-ErrorMessage"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.Message));
                    oresponse.Headers["MIS-ErrorStackTrace"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(ex.ToString()));
                    content = Encoding.UTF8.GetBytes("请求中出现错误");
                }
                else
                {
                    oresponse.ContentType = "text/json;charset=utf-8";
                    oresponse.StatusCode = 200;
                    if (request.Result != null)
                    {
                        content = Encoding.UTF8.GetBytes(response.GetActualResponse().ToJsonString());
                    }
                }
            }
            catch (Exception e)
            {
                content = Encoding.UTF8.GetBytes(e.ToString());
                owin.Response.ContentType = "text/plain;charset=utf-8";
                owin.Response.StatusCode = 500;
            }

            //输出内容,如果有
            if (content != null)
            {
                owin.Response.ContentLength = content.Length;
                await owin.Response.WriteAsync(content);
            }
        }




    }
}