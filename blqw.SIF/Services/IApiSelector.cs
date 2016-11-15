using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;

namespace blqw.SIF.Services
{
    /// <summary>
    /// 接口选择器
    /// </summary>
    public interface IApiSelector : IService
    {
        /// <summary>
        /// 根据请求
        /// </summary>
        /// <param name="request"> api请求 </param>
        /// <param name="container"> api所属容器 </param>
        /// <returns></returns>
        ApiDescriptor Select(IApiRequest request, Container container);
    }
}
