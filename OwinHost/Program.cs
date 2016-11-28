using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace OwinHost
{
    class Program
    {
        static void Main(string[] args)
        {
            StartOptions options = new StartOptions();

            //服务器Url设置
            options.Urls.Add("http://localhost:3398");
            //options.Urls.Add("http://192.168.1.1:3398");

            //Server实现类库设置
            options.ServerFactory = "Microsoft.Owin.Host.HttpListener";
            //以当前的Options和Startup启动Server
            using (WebApp.Start(options, Startup))
            {
                //显示启动信息,通过ReadLine驻留当前进程
                Console.WriteLine("Owin Host/Server started,press enter to exit it...");
                Console.ReadLine();
            }//Server在Dispose中关闭
        }

        private static void Startup(Owin.IAppBuilder app)
        {
            //加载Sample Middleware
            Console.WriteLine("Sample Middleware loaded...");
            app.Use<SampleMiddleware>();
        }
    }
}
