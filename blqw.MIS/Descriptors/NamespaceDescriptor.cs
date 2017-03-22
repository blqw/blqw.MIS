using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 用于描述命名空间
    /// </summary>
    public class NamespaceDescriptor : DescriptorBase
    {
        /// <summary>
        /// 实例化命名空间
        /// </summary>
        /// <param name="fullName">命名空间完整名称</param>
        public NamespaceDescriptor(string fullName, IList<ApiClassDescriptor> apiClasses)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Name = fullName.Split('.').Last();
            Types = apiClasses.AsReadOnly();
        }

        /// <summary>
        /// 命名空间完整名称
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// API类型只读集合
        /// </summary>
        public IReadOnlyList<ApiClassDescriptor> Types { get; }
        
    }
}
