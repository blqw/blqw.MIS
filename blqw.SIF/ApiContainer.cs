using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;
using blqw.SIF.Services;
using System.Reflection;

namespace blqw.SIF
{
    /// <summary>
    /// 容器
    /// </summary>
    public sealed class ApiContainer
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="id">容器名称,唯一标识</param>
        public ApiContainer(string id, IApiContainerServices serviceProvider)
        {
            ID = id;
            Services = serviceProvider;
            Apis = new ApiCollection(this);
        }

        public string ID { get; }
        
        /// <summary>
        /// 接口集合
        /// </summary>
        public ApiCollection Apis { get; }

        /// <summary>
        /// 服务集合
        /// </summary>
        public IApiContainerServices Services { get; }

        }
}

