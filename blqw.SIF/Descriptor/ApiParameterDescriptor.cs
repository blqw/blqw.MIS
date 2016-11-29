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
    public sealed class ApiParameterDescriptor
    {
        /// <summary>
        ///     初始化接口参数描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="parameter"></param>
        public ApiParameterDescriptor(ParameterInfo parameter)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Name = parameter.Name;
            ParameterType = parameter.ParameterType;
            var defattr = Attribute.GetCustomAttribute(parameter, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
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
    }
}