using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Owin
{
    class OwinProvider : ApiServiceProvider
    {
        public override Assembly[] Assemblies 
            => AppDomain.CurrentDomain.GetAssemblies();
    }
}
