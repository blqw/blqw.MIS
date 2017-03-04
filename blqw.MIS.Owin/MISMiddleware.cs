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
            var urls = ((Selector) entry.Selector).GetAllUrls().ToArray();
            foreach (var url in urls)
            {
                Console.WriteLine(url);
            }
            Console.WriteLine($"载入接口完成,共 {urls.Length} 个");
        }


        public override async Task Invoke(IOwinContext owin)
        {
            var request = new Request(owin);
            Exception exception = null;
            try
            {
                var response = await _scheduler.OnExecuteAsync(new RequestSetter(request));
                if (response == null)
                {
                    await Next.Invoke(owin);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await owin.Response.WriteErrorAsync(exception);
                request.Result = exception;
                _scheduler.OnError(request, exception);
            }

            _scheduler.OnEnd(request);
        }




    }
}