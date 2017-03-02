using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// API参数
    /// </summary>
    public struct ApiArgument
    {
        public ApiArgument(ParameterInfo parameter, object value)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            Parameter = parameter;
            Value = value;
        }

        /// <summary>
        /// 参数
        /// </summary>
        public ParameterInfo Parameter { get; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
    }
}
