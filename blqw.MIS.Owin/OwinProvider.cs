using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Services;
using System.IO;
using blqw.MIS.DataModification;
using blqw.MIS.Filters;
using blqw.MIS.NetFramework45;
using blqw.MIS.Validation;

namespace blqw.MIS.OwinAdapter
{
    class OwinProvider : ApiContainerProvider
    {
        public OwinProvider() 
            : base("Owin")
        {
        }
    }
}
