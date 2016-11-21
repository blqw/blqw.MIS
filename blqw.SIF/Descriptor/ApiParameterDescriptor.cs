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
        public ApiParameterDescriptor(ApiDescriptor api, ParameterInfo parameter)
        {
            if (api == null) throw new ArgumentNullException(nameof(api));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            Api = api;
            Parameter = parameter;
            Name = parameter.Name;
            var defattr = Attribute.GetCustomAttribute(parameter, typeof(DefaultValueAttribute), true) as DefaultValueAttribute;
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
            }
            else if (parameter.IsOptional)
            {
                DefaultValue = parameter.DefaultValue;
            }
            Validations = new ReadOnlyCollection<DataValidationAttribute>((DataValidationAttribute[])parameter.GetCustomAttributes(typeof(DataValidationAttribute), true));
        }

        /// <summary>
        ///     接口
        /// </summary>
        public ApiDescriptor Api { get; }

        /// <summary>
        ///     接口参数
        /// </summary>
        public ParameterInfo Parameter { get; }


        /// <summary>
        ///     当前参数的验证特性
        /// </summary>
        public ICollection<DataValidationAttribute> Validations { get; }

        public object DefaultValue { get; }
        public string Name { get; }

        /// <summary>
        ///     检查数据字段的值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="items"> 与要验证的值关联的键/值对的字典</param>
        /// <returns></returns>
        public ApiException IsValid(object value, IDictionary<string, object> items)
            => (from it in Validations
                where it.IsValid(value, items) == false
                select new ApiException(it.ErrorCode, it.GetErrorMessage(value, items))
            ).FirstOrDefault();
    }
}