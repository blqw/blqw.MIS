﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 用于描述API属性
    /// </summary>
    public class ApiPropertyDescriptor : DescriptorBase
    {
        /// <summary>
        ///     初始化接口属性描述
        /// </summary>
        /// <param name="property"></param>
        /// <param name="apiclass"></param>
        public ApiPropertyDescriptor(PropertyInfo property, ApiClassDescriptor apiclass)
            : base((apiclass ?? throw new ArgumentNullException(nameof(apiclass))).Container)
        {
            ApiClass = apiclass;
            CheckProperty(property, true);
            Property = property;
            Name = property.Name;
            PropertyType = property.PropertyType;
            var defattr = property.GetCustomAttribute<DefaultValueAttribute>(true);
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
                HasDefaultValue = true;
            }


            var setter = (IServiceProvider)Activator.CreateInstance(typeof(SetterProvider<,>).GetTypeInfo().MakeGenericType(property.DeclaringType, property.PropertyType), property);
            Setter = (Action<object, object>)setter.GetService(typeof(Action<object, object>));
        }

        /// <summary>
        /// 接口属性
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// API属性的设置值
        /// </summary>
        internal Action<object, object> Setter { get; }

        /// <summary>
        /// 属性默认值
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// 是否存在默认值
        /// </summary>
        public bool HasDefaultValue { get; }

        /// <summary>
        /// API 属性类型
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// API 所在类
        /// </summary>
        public ApiClassDescriptor ApiClass { get; }

        /// <summary>
        /// 检查属性合法性
        /// </summary>
        /// <param name="property">待检查的属性</param>
        /// <param name="throwIfError">是否抛出异常</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
        private static bool CheckProperty(PropertyInfo property, bool throwIfError)
        {
            if (property == null)
            {
                return throwIfError ? throw new ArgumentNullException(nameof(property)) : false;
            }
            var setter = property.SetMethod;
            if (setter == null)
            {
                return throwIfError ? throw new InvalidOperationException("未找到该属性的 set 访问器") : false;
            }
            if (setter.IsPublic == false)
            {
                return throwIfError ? throw new InvalidOperationException("受保护的属性不能用作API属性") : false;
            }
            if (setter.IsStatic)
            {
                return throwIfError ? throw new InvalidOperationException("静态属性不能用作API属性") : false;
            }
            if (setter.IsAbstract)
            {
                return throwIfError ? throw new InvalidOperationException("抽象属性不能用作API属性") : false;
            }
            if (setter.ContainsGenericParameters)
            {
                return throwIfError ? throw new InvalidOperationException("开放式泛型类型属性不能用作API属性") : false;
            }

            if (property.GetIndexParameters()?.Length == 0)
            {
                return throwIfError ? throw new InvalidOperationException("索引器不能声明为API属性") : false;
            }
            return true;
        }

        /// <summary>
        /// 创建API属性描述,如果属性不合法,则返null
        /// </summary>
        /// <param name="property"></param>
        /// <param name="apiclass"></param>
        /// <returns></returns>
        internal static ApiPropertyDescriptor Create(PropertyInfo property, ApiClassDescriptor apiclass)
            => CheckProperty(property, false) ? new ApiPropertyDescriptor(property, apiclass) : null;

        /// <summary>
        /// 属性设置委托的提供程序
        /// </summary>
        /// <typeparam name="T">需要设置值的属性的所在类型</typeparam>
        /// <typeparam name="TProperty">需要设置属性值的属性的类型</typeparam>
        private class SetterProvider<T, TProperty> : IServiceProvider
        {

            public SetterProvider(PropertyInfo property)
                => _set = (Action<T, TProperty>)property.SetMethod.CreateDelegate(typeof(Action<T, TProperty>));

            private readonly Action<T, TProperty> _set;

            private void SetValue(object instance, object value)
                => _set((T)instance, (TProperty)value);

            public object GetService(Type serviceType)
                => (Action<object, object>)SetValue;
        }
    }
}
