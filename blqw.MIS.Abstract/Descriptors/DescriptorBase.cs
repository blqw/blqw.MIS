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
        protected DescriptorBase(ApiContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Extends = new NameDictionary();
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// API容器
        /// </summary>
        public ApiContainer Container { get; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public IDictionary<string, object> Extends { get; }
    }
}
