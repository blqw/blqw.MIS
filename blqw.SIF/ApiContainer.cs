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
        public ApiContainer(string id, ApiServiceProvider serviceProvider)
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
        public ApiServiceProvider Services { get; }


        /// <summary>
        /// 如果Api是静态方法,则该方法返回null
        /// <para></para>如果Api所在类是抽象类,则返回null,如果<seealso cref="IApiDataProvider"/>提供的参数无法正确调用构造函数,则抛出异常
        /// <para></para>否则返回指定Api所在类的实例,
        /// </summary>
        /// <param name="api">Api描述</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public object CreateApiInstance(ApiDescriptor api, IApiDataProvider dataProvider)
            => api.Method.IsStatic || api.ApiClass.Type.GetTypeInfo().IsAbstract == true ? null : Activator.CreateInstance(api.ApiClass.Type);
    }
}

