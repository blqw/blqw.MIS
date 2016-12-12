using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace blqw.UIF.Descriptor
{
    /// <summary>
    /// 用于描述一个命名空间
    /// </summary>
    public class NamespaceDescriptor:IDescriptor
    {
        private readonly List<ApiClassDescriptor> _types;

        public NamespaceDescriptor(string @namespace, ApiContainer container)
        {
            FullName = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Name = @namespace.Split('.').Last();
            Settings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            _types = new List<ApiClassDescriptor>();
            Types = new ReadOnlyCollection<ApiClassDescriptor>(_types);
        }
        /// <summary>
        /// 命名空间名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 命名空间完整名称
        /// </summary>
        public string FullName { get; }
        public ReadOnlyCollection<ApiClassDescriptor> Types { get; }

        public ApiContainer Container { get; }

        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 添加一个ApiClass描述
        /// </summary>
        /// <param name="apiclass"></param>
        internal void AddApiCalss(ApiClassDescriptor apiclass)
            => _types.Add(apiclass);
    }
}
