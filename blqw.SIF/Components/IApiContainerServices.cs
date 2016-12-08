using blqw.SIF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF
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
        /// 所有可能存在API方法的类型
        /// </summary>
        IEnumerable<Type> ApiTypes { get; }

        /// <summary>
        /// 设置解析器
        /// </summary>
        IApiSettingParser SettingParser { get; }

        /// <summary>
        /// 类型转换器
        /// </summary>
        IConverter Converter { get; }

    }
}
