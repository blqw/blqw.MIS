using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Events
{
    public class ApiEventArgs : EventArgs
    {
        public ApiEventArgs(ApiCallContext context)
        {
            Context = context;
        }
        
        public ApiCallContext Context { get; }
    }
}
