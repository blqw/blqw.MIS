using blqw.UIF.DataModification;
using blqw.UIF.Filters;
using blqw.UIF.Services;
using blqw.UIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.UIF
{
    /// <summary>
    /// Api容器服务
    /// </summary>
    public interface IApiContainerServices
    {
        /// <summary>
        /// 容器ID
        /// </summary>
        string ContainerId { get; }

        /// <summary>
        /// 获取定义类型的集合
        /// </summary>
        IEnumerable<Type> DefinedTypes { get; }

        /// <summary>
        /// 设置解析器
        /// </summary>
        IApiSettingParser SettingParser { get; }

        /// <summary>
        /// 类型转换器
        /// </summary>
        IConverter Converter { get; }
        
        IEnumerable<ApiFilterAttribute> GlobalFilters { get; }

        IEnumerable<DataValidationAttribute> GlobalValidations { get; }

        IEnumerable<DataModificationAttribute> GlobalModifications { get; }

    }
}
