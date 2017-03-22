using System.Web.Routing;

namespace blqw.MIS.MVC
{
    public class MISRoute : Route
    {
        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="urlTemplate"></param>
        public static void Register(string routeName = "mis", string urlTemplate= "mis/{class}/{method}")
        {
            RouteTable.Routes.Add(routeName, new MISRoute(urlTemplate));
        }

        public MISRoute(string url = "mis/{class}/{method}")
            : base(url, new HttpRouteHandler(url))
        {
        }
    }
}
