using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using blqw.MIS.Validation;
using blqw.MIS.DataModification;

namespace blqw.MIS.Descriptor
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
        public ApiParameterDescriptor(ApiClassDescriptor apiclass, ParameterInfo parameter, ApiContainer container)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            ApiClass = apiclass ?? throw new ArgumentNullException(nameof(apiclass));

            Name = parameter.Name;
            ParameterType = parameter.ParameterType;

            var defattr = parameter.GetCustomAttribute<DefaultValueAttribute>(true);
            if (defattr != null)
            {
                DefaultValue = defattr.Value;
                HasDefaultValue = true;
            }
            else if (parameter.HasDefaultValue)
            {
                DefaultValue = parameter.DefaultValue;
                HasDefaultValue = true;
            }

            var attrs = parameter.GetCustomAttributes<ApiParameterAttribute>();
            Settings = container.Provider.ParseSetting(attrs) ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);


            var validations = new List<DataValidationAttribute>();
            foreach (DataValidationAttribute filter in parameter.GetCustomAttributes<DataValidationAttribute>().Reverse()
                        .Union(Parameter.Member.DeclaringType.GetTypeInfo().GetCustomAttributes<DataValidationAttribute>().Reverse())
                        .Union(container.Validations.Reverse()))
            {
                if (validations.Any(a => a.Match(filter)) == false && filter.IsAllowType(ParameterType))
                {
                    validations.Add(filter);
                }
            }
            validations.Reverse();
            DataValidations = validations.AsReadOnly();

            var modifications = new List<DataModificationAttribute>();
            foreach (DataModificationAttribute filter in parameter.GetCustomAttributes<DataModificationAttribute>().Reverse()
                        .Union(Parameter.Member.DeclaringType.GetTypeInfo().GetCustomAttributes<DataModificationAttribute>().Reverse())
                        .Union(container.Modifications.Reverse()))
            {
                if (modifications.Any(a => a.Match(filter)) == false && filter.IsAllowType(ParameterType))
                {
                    modifications.Add(filter);
                }
            }
            modifications.Reverse();
            DataModifications = modifications.AsReadOnly();
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
        /// 是否存在默认值
        /// </summary>
        public bool HasDefaultValue { get; }

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

        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 数据验证规则
        /// </summary>
        public ICollection<DataValidationAttribute> DataValidations { get; }
        /// <summary>
        /// 数据更改规则
        /// </summary>
        public ICollection<DataModificationAttribute> DataModifications { get; }

    }
}