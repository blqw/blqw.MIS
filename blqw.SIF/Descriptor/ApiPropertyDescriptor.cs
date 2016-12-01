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
    public sealed class ApiPropertyDescriptor: IDescriptor
    {
        /// <summary>
        ///     初始化接口属性描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="property"></param>
        private ApiPropertyDescriptor(ApiClassDescriptor apiclass, PropertyInfo property, ApiContainer container, ApiSettings settings)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ApiClass = apiclass ?? throw new ArgumentNullException(nameof(apiclass));
            
            Name = property.Name;
            var defattr = property.GetCustomAttribute<DefaultValueAttribute>(true);
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

        public ApiContainer Container { get; }

        public ApiSettings Settings { get; }

        public ApiClassDescriptor ApiClass { get; }

        internal static ApiPropertyDescriptor Create(PropertyInfo p, ApiContainer container, ApiClassDescriptor apiclass)
        {
            var attrs = p.GetCustomAttributes<ApiPropertyAttribute>().Where(it => it.Container == null || it.Container == container.ID).OrderBy(it => it.Container).ToArray();
            if (attrs.Length == 0)
            {
                return null;
            }
            var settings = container.Services.ParseSetting(attrs);
            return new ApiPropertyDescriptor(apiclass, p, container, settings);
        }
        
    }
}
