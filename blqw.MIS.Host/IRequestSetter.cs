using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS
{
    public interface IRequestSetter
    {
        /// <summary>
        /// 参数
        /// </summary>
        IReadOnlyList<ApiArgument> Arguments { set; }
        /// <summary>
        /// 属性
        /// </summary>
        IReadOnlyList<ApiProperty> Properties { set; }

        /// <summary>
        /// 获取或设置请求的返回值
        /// </summary>
        object Result { set; }
        /// <summary>
        /// API实例
        /// </summary>
        object Instance { set; }
        /// <summary>
        /// API描述
        /// </summary>
        ApiDescriptor ApiDescriptor { set; }
        /// <summary>
        /// 请求实例
        /// </summary>
        IRequest Request { get; }
    }
}
