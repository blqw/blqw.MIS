using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptor;

namespace blqw.MIS.MVCAdapter
{
    internal static class ApiExtensions
    {
        public static void SetHasBody(this ApiDescriptor api)
        {
            if (api != null)
                api.Settings["HasBody"] = true;
        }

        public static bool HasBody(this ApiDescriptor api)
            => api?.Settings["HasBody"] is bool b && b;
    }
}
