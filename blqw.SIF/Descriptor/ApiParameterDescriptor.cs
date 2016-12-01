using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using blqw.SIF.Validation;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    ///     用于描述一个接口参数
    /// </summary>
    public sealed class ApiParameterDescriptor : IDescriptor
    {

        /// <summary>
        ///     初始化接口参数描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="parameter"></param>
        public ApiParameterDescriptor(ApiClassDescriptor apiclass, ParameterInfo parameter, ApiContainer container, ApiSettings settings)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ApiClass = apiclass ?? throw new ArgumentNullException(nameof(apiclass));

            Name = parameter.Name;
            ParameterType = parameter.ParameterType;

            var defattr = parameter.GetCustomAttribute<DefaultValueAttribute>(true);
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
            }
            else if (parameter.IsOptional)
            {
                DefaultValue = parameter.DefaultValue;
            }
        }

        /// <summary>
        ///     接口参数
        /// </summary>
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// 参数默认值
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ParameterType { get; }

        public ApiClassDescriptor ApiClass { get; }

        public ApiContainer Container { get; }

        public ApiSettings Settings { get; }
    }
}