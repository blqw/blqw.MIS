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
        /// API类型描述
        /// </summary>
        private readonly List<ApiClassDescriptor> _types = new List<ApiClassDescriptor>();

        /// <summary>
        /// 实例化命名空间
        /// </summary>
        /// <param name="fullName">命名空间完整名称</param>
        /// <param name="container">API容器</param>
        public NamespaceDescriptor(string fullName, ApiContainer container) 
            : base(container)
        {
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            Name = fullName.Split('.').Last();
            Types = _types.AsReadOnly();
        }

        /// <summary>
        /// 命名空间完整名称
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// API类型只读集合
        /// </summary>
        public IReadOnlyList<ApiClassDescriptor> Types { get; }
        
        /// <summary>
        /// 添加一个ApiClass描述
        /// </summary>
        /// <param name="apiclass"></param>
        internal void AddApiCalss(ApiClassDescriptor apiclass)
            => _types.Add(apiclass);
    }
}
