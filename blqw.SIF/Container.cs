using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;
using blqw.SIF.Services;

namespace blqw.SIF
{
    /// <summary>
    /// 容器
    /// </summary>
    public sealed class Container
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="id">容器名称,唯一标识</param>
        public Container(string id)
        {
            
        }

        /// <summary>
        /// 接口集合
        /// </summary>
        public ICollection<ApiDescriptor> Apis { get; }

        /// <summary>
        /// 服务集合
        /// </summary>
        public ServiceCollection Services { get; }


    }
}
