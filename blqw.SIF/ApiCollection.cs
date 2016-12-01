using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Descriptor;
using System.Collections.ObjectModel;
using System.Reflection;

namespace blqw.SIF
{
    /// <summary>
    /// Api集合
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

            Namespaces = new ReadOnlyCollection<NamespaceDescriptor>(_namespaces);
            ApiClasses = new ReadOnlyCollection<ApiClassDescriptor>(_types);
            Apis = new ReadOnlyCollection<ApiDescriptor>(_apis);

            var assemblies = ApiServiceProvider.GetService(container.Services.Assemblies);
            FindAllApis(assemblies);
        }

        private void FindAllApis(Assembly[] assemblies)
        {
            foreach (var a in assemblies)
            {
                foreach (var t in a.ExportedTypes)
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
        }

        public ICollection<NamespaceDescriptor> Namespaces { get; }
        public ICollection<ApiClassDescriptor> ApiClasses { get; }
        public ICollection<ApiDescriptor> Apis { get; }
        public ApiContainer Container { get; }

    }
}
