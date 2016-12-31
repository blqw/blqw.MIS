using blqw.MIS.DataModification;
using blqw.MIS.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
        public ApiParameterDescriptor(ParameterInfo parameter, ApiDescriptor api)
        {
            Api = api ?? throw new ArgumentNullException(nameof(api));
            Container = api.Container;
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));

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
            Settings = Container.Provider.ParseSetting(attrs) ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);


            var validations = new List<DataValidationAttribute>();
            foreach (DataValidationAttribute filter in parameter.GetCustomAttributes<DataValidationAttribute>().Reverse()
                        .Union(Parameter.Member.DeclaringType.GetTypeInfo().GetCustomAttributes<DataValidationAttribute>().Reverse())
                        .Union(Container.Validations.Reverse()))
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
                        .Union(Container.Modifications.Reverse()))
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
        /// <summary>
        /// API描述
        /// </summary>
        public ApiDescriptor Api { get; }

        /// <summary>
        /// API容器
        /// </summary>
        public ApiContainer Container { get; }

        /// <summary>
        /// API设置
        /// </summary>
        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 数据验证只读规则
        /// </summary>
        public IReadOnlyList<DataValidationAttribute> DataValidations { get; }

        /// <summary>
        /// 数据更改只读规则
        /// </summary>
        public IReadOnlyList<DataModificationAttribute> DataModifications { get; }

    }
}