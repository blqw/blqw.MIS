using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    /// 用于描述一个命名空间
    /// </summary>
    public class NamespaceDescriptor
    {
        /// <summary>
        /// 命名空间名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 命名空间完整名称
        /// </summary>
        public string FullName { get; }
    }
}
