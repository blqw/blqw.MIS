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
                var result = await _scheduler.ExecuteAsync(new RequestSetter(request));
                var response = request.OwinContext.Response;
                response.Expires = DateTimeOffset.Now;
                response.Headers["MIS-RequestId"] = request.Id;
                if (result is Exception ex)
                {
                    ex = ex.ProcessException();
                    if (result is ApiNotFoundException)
                    {
                        await Next.Invoke(owin);
                        if (response.StatusCode == 404)
                        {
                            response.ContentType = "text/plain;charset=utf-8";
                            content = Encoding.UTF8.GetBytes("api不存在");
                        }
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