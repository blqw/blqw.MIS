using blqw.MIS.MVCAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace blqw.MIS
{
    public class MISRoute : Route
    {
        public MISRoute(string url = "api/{api_class}/{api_method}")
            : base(url, new HttpRouteHandler())
        {
            _container = new ApiContainer(new MVC5Provider());
            foreach (var api in _container.ApiCollection.Apis)
            {
                if (api.Parameters.Any(p => p.Name.Equals("body", StringComparison.OrdinalIgnoreCase)))
                {
                    api.SetHasBody();
                }
            }
        }

        private readonly ApiContainer _container;

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var route = base.GetRouteData(httpContext);
            if (route == null)
            {
                return null;
            }

            var ns = route.Values["api_namespace"] as string;
            var cls = route.Values["api_class"] as string;
            var msd = route.Values["api_method"] as string;

            var @namespace = ns == null ? null : _container.ApiCollection.Namespaces.FirstOrDefault(n => ns.Equals(n.FullName, StringComparison.OrdinalIgnoreCase));
            var type = (@namespace?.Types ?? _container.ApiCollection.ApiClasses).FirstOrDefault(t => cls.Equals(t.FullName, StringComparison.OrdinalIgnoreCase));
            var api = type?.Apis.FirstOrDefault(a => msd.Equals(a.Name, StringComparison.OrdinalIgnoreCase));

            if (api == null)
            {
                return null;
            }
            _container.OnBeginRequest();
            route.DataTokens["mis.api"] = api;
            return route;
        }
    }
}
