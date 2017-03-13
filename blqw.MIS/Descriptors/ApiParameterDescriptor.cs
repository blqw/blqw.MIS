using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 用于描述API参数
    /// </summary>
    public class ApiParameterDescriptor : DescriptorBase
    {

        /// <summary>
        /// 初始化接口参数描述
        /// </summary>
        /// <param name="api"></param>
        /// <param name="parameter"></param>
        public ApiParameterDescriptor(ParameterInfo parameter, ApiDescriptor api) 
        {
            Api = api;
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
            IsEntity = ParameterType.Namespace != "System";
            if (IsEntity)
            {
                Properties = ParameterType.GetRuntimeProperties().Select(p => new ApiPropertyDescriptor(p)).AsReadOnly();
            }
        }

        /// <summary>
        /// 接口参数
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
        /// 参数类型
        /// </summary>
        public Type ParameterType { get; }

        /// <summary>
        /// API描述
        /// </summary>
        public ApiDescriptor Api { get; }
        
        /// <summary>
        /// 参数是否为一个实体
        /// </summary>
        public bool IsEntity { get; }

        /// <summary>
        /// 属性描述只读集合
        /// </summary>
        public IReadOnlyList<ApiPropertyDescriptor> Properties { get; }
    }
}