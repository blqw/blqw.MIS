using System.Collections.Generic;

namespace blqw.MIS.Descriptors
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

        /// <summary>
        /// 扩展属性
        /// </summary>
        IDictionary<string, object> Extends { get; }
    }
}
