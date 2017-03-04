using blqw.MIS.Descriptors;
using Microsoft.Owin;
using System;
using System.Collections.Generic;

namespace blqw.MIS.Owin
{
    public class RouteTable
    {
        private readonly Dictionary<string, ApiDescriptor> _static;

        public RouteTable(IEnumerable<ApiDescriptor> apis)
        {
            _static = new Dictionary<string, ApiDescriptor>(StringComparer.OrdinalIgnoreCase);
            foreach (var api in apis)
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

        public IEnumerable<string> Urls => _static.Keys;
    }
}
