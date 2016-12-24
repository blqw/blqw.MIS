using blqw.MIS.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using blqw.MIS.Validation;
using blqw.MIS.DataModification;

namespace blqw.MIS.Descriptor
{
    /// <summary>
    /// 用于描述一个API属性
    /// </summary>
    public sealed class ApiPropertyDescriptor : IDescriptor
    {
        /// <summary>
        ///     初始化接口属性描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="property"></param>
        private ApiPropertyDescriptor(PropertyInfo property, ApiClassDescriptor apiclass, IDictionary<string, object> settings)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ApiClass = apiclass ?? throw new ArgumentNullException(nameof(apiclass));
            Container = apiclass.Container;

            Name = property.Name;
            PropertyType = property.PropertyType;
            var defattr = property.GetCustomAttribute<DefaultValueAttribute>(true);
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
                HasDefaultValue = true;
            }
            

            var setter = (IServiceProvider)Activator.CreateInstance(typeof(SetProvider<,>).GetTypeInfo().MakeGenericType(property.DeclaringType, property.PropertyType), property);
            Setter = (Action<object, object>)setter.GetService(typeof(Action<object, object>));


            var validations = new List<DataValidationAttribute>();
            foreach (DataValidationAttribute filter in property.GetCustomAttributes<DataValidationAttribute>().Reverse()
                        .Union(property.DeclaringType.GetTypeInfo().GetCustomAttributes<DataValidationAttribute>().Reverse())
                        .Union(Container.Validations.Reverse()))
            {
                if (validations.Any(a => a.Match(filter)) == false && filter.IsAllowType(PropertyType))
                {
                    validations.Add(filter);
                }
            }
            validations.Reverse();
            DataValidations = validations.AsReadOnly();

            var modifications = new List<DataModificationAttribute>();
            foreach (DataModificationAttribute filter in property.GetCustomAttributes<DataModificationAttribute>().Reverse()
                        .Union(property.DeclaringType.GetTypeInfo().GetCustomAttributes<DataModificationAttribute>().Reverse())
                        .Union(Container.Modifications.Reverse()))
            {
                if (modifications.Any(a => a.Match(filter)) == false && filter.IsAllowType(PropertyType))
                {
                    modifications.Add(filter);
                }
            }
            modifications.Reverse();
            DataModifications = modifications.AsReadOnly();
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
        /// 是否存在默认值
        /// </summary>
        public bool HasDefaultValue { get; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// API 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// API 容器
        /// </summary>
        public ApiContainer Container { get; }

        /// <summary>
        /// API 设置
        /// </summary>
        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// API 所在类
        /// </summary>
        public ApiClassDescriptor ApiClass { get; }


        /// <summary>
        /// 数据验证规则
        /// </summary>
        public ICollection<DataValidationAttribute> DataValidations { get; }
        /// <summary>
        /// 数据更改规则
        /// </summary>
        public ICollection<DataModificationAttribute> DataModifications { get; }

        internal static ApiPropertyDescriptor Create(PropertyInfo property, ApiClassDescriptor apiclass)
        {
            if (property.CanWrite && property.SetMethod.IsPublic && property.GetIndexParameters().Length == 0)
            {
                var attrs = property.GetCustomAttributes<ApiPropertyAttribute>();
                var settings = apiclass.Container.Provider.ParseSetting(attrs);
                if (settings == null)
                {
                    return null;
                }
                return new ApiPropertyDescriptor(property, apiclass, settings);
            }
            return null;
        }

        class SetProvider<I, V> : IServiceProvider
        {
            public SetProvider(PropertyInfo property)
            {
                _set = (Action<I, V>)property.SetMethod.CreateDelegate(typeof(Action<I, V>));
            }

            private readonly Action<I, V> _set;
            private void SetValue(object instance, object value)
                => _set((I)instance, (V)value);

            public object GetService(Type serviceType)
                => (Action<object, object>)SetValue;
        }
    }
}
