using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 描述基类
    /// </summary>
    public abstract class DescriptorBase : IDescriptor
    {
        protected DescriptorBase()
        {
            Extends = new NameDictionary();
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public IDictionary<string, object> Extends { get; }
    }
}
