using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 提供创建API类描述的功能定义
    /// </summary>
    public interface IApiClassDescriptorFactory : IService
    {
        /// <summary>
        /// 创建API类描述
        /// </summary>
        /// <param name="type">类型对象</param>
        /// <param name="container">容器</param>
        /// <returns></returns>
        ApiClassDescriptor Create(Type type, ApiContainer container);
    }
}
