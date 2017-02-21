using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace blqw.MIS.Descriptor
{
    /// <summary>
    /// 用于描述一个命名空间
    /// </summary>
    public class NamespaceDescriptor : IDescriptor
    {
        /// <summary>
        /// API类型描述
        /// </summary>
        private readonly List<ApiClassDescriptor> _types;

        /// <summary>
        /// 实例化命名空间
        /// </summary>
        /// <param name="namespace">命名空间完整名称</param>
        /// <param name="container">API容器</param>
        public NamespaceDescriptor(string @namespace, ApiContainer container)
        {
            FullName = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Name = @namespace.Split('.').Last();
            Settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _types = new List<ApiClassDescriptor>();
            Types = new ReadOnlyCollection<ApiClassDescriptor>(_types);
            IFormattable
        }
        /// <summary>
        /// 命名空间名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 命名空间完整名称
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// API类型只读集合
        /// </summary>
        public IReadOnlyList<ApiClassDescriptor> Types { get; }

        /// <summary>
        /// API容器
        /// </summary>
        public ApiContainer Container { get; }

        /// <summary>
        /// API设置
        /// </summary>
        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 添加一个ApiClass描述
        /// </summary>
        /// <param name="apiclass"></param>
        internal void AddApiCalss(ApiClassDescriptor apiclass)
            => _types.Add(apiclass);
    }
}
