using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.Services;
using System.IO;
using blqw.UIF.DataModification;
using blqw.UIF.Filters;
using blqw.UIF.NetFramework45;
using blqw.UIF.Validation;

namespace blqw.UIF.Owin
{
    class OwinProvider : ApiContainerProvider
    {
        public OwinProvider() 
            : base("Owin")
        {
        }
    }
}
