using System;
using System.Collections.Generic;

namespace blqw.UIF.Services
{
    /// <summary>
    /// API服务标准接口
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 服务属性集
        /// </summary>
        IDictionary<string, object> Data { get; }

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        bool RequireClone { get; }

        /// <summary>
        /// 克隆当前对象,当<see cref="RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        IService Clone();
    }
}