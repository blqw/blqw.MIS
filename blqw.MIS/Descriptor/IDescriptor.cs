using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Descriptor
{
    /// <summary>
    /// 描述接口
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        ApiContainer Container { get; }

        IDictionary<string, object> Settings { get; }
    }
}
