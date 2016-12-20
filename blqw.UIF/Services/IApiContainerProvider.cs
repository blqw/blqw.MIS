using blqw.UIF.DataModification;
using blqw.UIF.Filters;
using blqw.UIF.Services;
using blqw.UIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using blqw.UIF.Descriptor;
using blqw.UIF.Logging;

namespace blqw.UIF
{
    /// <summary>
    /// Api容器服务
    /// </summary>
    public interface IApiContainerProvider
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

        /// <summary>
        /// 日志服务
        /// </summary>
        ILoggerService Logger { get; }

        /// <summary>
        /// 全局过滤器
        /// </summary>
        IEnumerable<ApiFilterAttribute> GlobalFilters { get; }

        /// <summary>
        /// 全局验证器
        /// </summary>
        IEnumerable<DataValidationAttribute> GlobalValidations { get; }

        /// <summary>
        /// 全局数据修改器
        /// </summary>
        IEnumerable<DataModificationAttribute> GlobalModifications { get; }

        /// <summary>
        /// 创建返回值更新器
        /// </summary>
        /// <returns></returns>
        IResultUpdater CreateResultUpdater();
        
    }
}
