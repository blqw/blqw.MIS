using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF
{
    class ParseResult
    {
        public object Instance { get; set; }

        public MethodInfo Method { get; set; }

        public IList<object> Parameters { get; set; }

        public IDictionary<MemberInfo, object> Properties { get; set; }
    }
}
