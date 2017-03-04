using System.Web.Routing;

namespace blqw.MIS.MVC
{
    public class MISRoute : Route
    {
        public MISRoute(string url = "mis/{class}/{method}")
            : base(url, new HttpRouteHandler(url))
        {
        }
    }
}
