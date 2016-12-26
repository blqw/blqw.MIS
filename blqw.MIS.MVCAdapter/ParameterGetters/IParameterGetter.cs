using blqw.MIS.Descriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.MVCAdapter
{
    interface IParameterGetter
    {
        ApiData Get(ApiParameterDescriptor parameter);
    }
}
