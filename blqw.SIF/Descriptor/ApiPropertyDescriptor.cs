using blqw.SIF.Services;
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
    public sealed class ApiPropertyDescriptor : IDescriptor
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

            var setter = (IServiceProvider)Activator.CreateInstance(typeof(SetProvider<,>).GetTypeInfo().MakeGenericType(property.DeclaringType, property.PropertyType), property);
            Setter = (Action<object, object>)setter.GetService(typeof(Action<object, object>));
        }

        /// <summary>
        ///     接口属性
        /// </summary>
        public PropertyInfo Property { get; }


        internal readonly Action<object, object> Setter;


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
            if (p.CanWrite && p.SetMethod.IsPublic && p.GetIndexParameters().Length == 0)
            {
                var attrs = p.GetCustomAttributes<ApiPropertyAttribute>();
                var settings = container.Services.ParseSetting(attrs);
                if (settings == null)
                {
                    return null;
                }
                return new ApiPropertyDescriptor(apiclass, p, container, settings);
            }
            return null;
        }

        class SetProvider<I, V> : IServiceProvider
        {
            public SetProvider(PropertyInfo property)
            {
                _set = (Action<I, V>)property.SetMethod.CreateDelegate(typeof(Action<I, V>));
            }
            Action<I, V> _set;
            private void SetValue(object instance, object value)
                => _set((I)instance, (V)value);

            public object GetService(Type serviceType)
                => (Action<object, object>)SetValue;
        }
    }
}
