using blqw.MIS.Descriptors;
using blqw.MIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace blqw.MIS
{
    /// <summary>
    /// API容器
    /// </summary>
    public sealed class ApiContainer
    {
        private readonly List<NamespaceDescriptor> _namespaces;
        private readonly List<ApiClassDescriptor> _types;
        private readonly List<ApiDescriptor> _apis;

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="name">容器名称</param>
        /// <param name="exportedTypes">输出类型</param>
        /// <param name="factory">API类描述创建服务</param>
        public ApiContainer(string name, IEnumerable<Type> exportedTypes, IApiClassDescriptorFactory factory = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            _namespaces = new List<NamespaceDescriptor>();
            _types = new List<ApiClassDescriptor>();
            _apis = new List<ApiDescriptor>();

            FindAllApis(exportedTypes, factory);

            Namespaces = _namespaces.AsReadOnly();
            ApiClasses = _types.AsReadOnly();
            Apis = _apis.AsReadOnly();
        }

        /// <summary>
        /// 容器名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 命名空间集合
        /// </summary>
        public IReadOnlyList<NamespaceDescriptor> Namespaces { get; }
        /// <summary>
        /// API类集合
        /// </summary>
        public IReadOnlyList<ApiClassDescriptor> ApiClasses { get; }
        /// <summary>
        /// API集合
        /// </summary>
        public IReadOnlyList<ApiDescriptor> Apis { get; }


        /// <summary>
        /// 查找所有输出类型中的API
        /// </summary>
        /// <param name="exportedTypes">输出类型</param>
        /// <param name="factory">API类描述创建服务</param>
        private void FindAllApis(IEnumerable<Type> exportedTypes, IApiClassDescriptorFactory factory)
        {
            if (exportedTypes == null) throw new ArgumentNullException(nameof(exportedTypes));
            if (factory == null) factory = DefaultIApiClassDescriptorFactory.Instance;
            foreach (var t in exportedTypes)
            {
                var apiclass = factory.Create(t, this);
                if (apiclass == null)
                {
                    continue;
                }
                _types.Add(apiclass);
                var ns = _namespaces.FirstOrDefault(it => it.FullName == t.Namespace);
                if (ns == null)
                {
                    _namespaces.Add(ns = new NamespaceDescriptor(t.Namespace, this));
                }
                ns.AddApiCalss(apiclass);
                foreach (var type in _types)
                {
                    _apis.AddRange(type.Apis);
                }
            }
        }

    }
}

