using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.MIS.Descriptor;
using System.Collections.ObjectModel;
using System.Reflection;
using blqw.MIS.Services;

namespace blqw.MIS
{
    /// <summary>
    /// API集合
    /// </summary>
    public sealed class ApiCollection
    {
        readonly List<NamespaceDescriptor> _namespaces;
        readonly List<ApiClassDescriptor> _types;
        readonly List<ApiDescriptor> _apis;
        public ApiCollection(ApiContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));

            _namespaces = new List<NamespaceDescriptor>();
            _types = new List<ApiClassDescriptor>();
            _apis = new List<ApiDescriptor>();

            Namespaces = _namespaces.AsReadOnly();
            ApiClasses = _types.AsReadOnly();
            Apis = _apis.AsReadOnly();

            var types = container.Provider.DefinedTypes ?? throw new InvalidOperationException("Provider.DefinedTypes返回null");

            FindAllApis(types);
        }

        private void FindAllApis(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                var apiclass = ApiClassDescriptor.Create(t, Container);
                if (apiclass == null)
                {
                    continue;
                }
                _types.Add(apiclass);
                var ns = _namespaces.FirstOrDefault(it => it.FullName == t.Namespace);
                if (ns == null)
                {
                    _namespaces.Add(ns = new NamespaceDescriptor(t.Namespace, Container));
                }
                ns.AddApiCalss(apiclass);
                foreach (var type in _types)
                {
                    _apis.AddRange(type.Apis);
                }
            }
        }


        public IReadOnlyList<NamespaceDescriptor> Namespaces { get; }
        public IReadOnlyList<ApiClassDescriptor> ApiClasses { get; }
        public IReadOnlyList<ApiDescriptor> Apis { get; }
        public ApiContainer Container { get; }

    }
}
