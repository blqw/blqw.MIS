using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 默认的API类描述创建工厂
    /// </summary>
    public sealed class DefaultIApiClassDescriptorFactory : IApiClassDescriptorFactory
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static DefaultIApiClassDescriptorFactory Instance { get; } = new DefaultIApiClassDescriptorFactory();

        private DefaultIApiClassDescriptorFactory()
        {
            Name = "默认类描述创建工厂";
            Data = new NameDictionary();
            RequireClone = false;
        }


        public void Dispose()
        {
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; }

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public bool RequireClone { get; }

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public IService Clone() => new DefaultIApiClassDescriptorFactory();

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="type"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="container">Api容器</param>
        /// <returns></returns>
        public ApiClassDescriptor Create(Type type, ApiContainer container)
            => ApiClassDescriptor.Create(type, container);
    }
}
