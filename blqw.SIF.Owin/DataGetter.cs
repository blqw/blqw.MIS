using blqw.SIF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF.Descriptor;
using Microsoft.Owin;

namespace blqw.SIF.Owin
{
    class DataGetter : IApiDataGetter
    {
        public DataGetter(IOwinContext context)
        {

        }
        public bool TryGetParameter(ApiParameterDescriptor parameter, out object value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetProperty(string argName, out object value)
        {
            throw new NotImplementedException();
        }
    }
}
