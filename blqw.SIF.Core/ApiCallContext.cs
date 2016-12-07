using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    /// <summary>
    /// 当前Api调用上下文
    /// </summary>
    public class ApiCallContext
    {
        public IDictionary<string, object> Parameters { get; }
        public IDictionary<string, object> Properties { get; }
        public MethodInfo Method { get; }
        public object Result { get; }
        public Exception Exception { get; }
    }
}
