using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 表示基础请求接口
    /// </summary>
    public interface IRequestBase : IRequest
    {
        /// <summary>
        /// 接口描述
        /// </summary>
        ApiDescriptor ApiDescriptor { get; }
    }
}
