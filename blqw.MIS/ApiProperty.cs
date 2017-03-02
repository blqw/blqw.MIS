using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// API属性
    /// </summary>
    public struct ApiProperty
    {
        public ApiProperty(PropertyInfo property,object value)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            Property = property;
            Value = value;
        }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo Property { get; }
        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
    }
}
