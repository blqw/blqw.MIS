using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace blqw.MIS.MVCAdapter
{
    public class MISRoute : Route
    {
        public MISRoute(string url = "api/{apiclass}/{apimethod}")
            : base(url, new MISRouteHandler())
        {
            _container = new ApiContainer(new MVC5Provider());
        }

        private readonly ApiContainer _container;

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var route = base.GetRouteData(httpContext);
            if (route == null)
            {
                return null;
            }
            var ns = route.Values["apinamespace"] as string;
            var cls = route.Values["apiclass"] as string;
            var msd = route.Values["apimethod"] as string;

            var @namespace = ns == null ? null : _container.ApiCollection.Namespaces.FirstOrDefault(n => ns.Equals(n.FullName, StringComparison.OrdinalIgnoreCase));
            var type = (@namespace?.Types ?? _container.ApiCollection.ApiClasses).FirstOrDefault(t => cls.Equals(t.FullName, StringComparison.OrdinalIgnoreCase));
            var api = type?.Apis.FirstOrDefault(a=>msd.Equals(a.Name, StringComparison.OrdinalIgnoreCase));

            if (api == null)
            {
                return null;
            }
            _container.OnBeginRequest();
            route.DataTokens["mis.api"] = api;
            return route;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            return base.GetVirtualPath(requestContext, values);
        }

        protected override bool ProcessConstraint(HttpContextBase httpContext, object constraint, string parameterName, RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            return true;
        }
    }
}
