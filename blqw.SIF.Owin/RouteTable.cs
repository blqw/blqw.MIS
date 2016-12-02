using blqw.SIF.Descriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.Owin;

namespace blqw.SIF.Owin
{
    public class RouteTable
    {
        Dictionary<string, ApiDescriptor> _static;

        public RouteTable(ApiCollection apiCollection)
        {
            _static = new Dictionary<string, ApiDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (var api in apiCollection.Apis)
            {
                _static[$"/{api.ApiClass.Name}/{api.Name}"] = api;
            }
        }

        public ApiDescriptor Select(IOwinContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (_static.TryGetValue(context.Request.Uri.AbsolutePath, out var api))
            {
                return api;
            }
            return null;
        }
    }
}
