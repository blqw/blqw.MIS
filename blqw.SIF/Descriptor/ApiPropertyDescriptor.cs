using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    /// 用于描述一个Api属性
    /// </summary>
    public sealed class ApiPropertyDescriptor
    {
        /// <summary>
        ///     初始化接口属性描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="property"></param>
        public ApiPropertyDescriptor(PropertyInfo property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Name = property.Name;
            var defattr = Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
            }
        }

        /// <summary>
        ///     接口属性
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// 属性默认值
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string Name { get; }
        public Type PropertyType { get; set; }
    }
}
