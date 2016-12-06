using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ApiFilterAttribute : Attribute
    {

    }
}
