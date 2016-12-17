using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.UIF.Services;
using System.Reflection;
using System.Collections.ObjectModel;

namespace blqw.UIF.Descriptor
{
    /// <summary>
    /// 表示一个Api类
    /// </summary>
    public class ApiClassDescriptor : IDescriptor
    {
        private readonly List<ApiDescriptor> _apis;
        private readonly List<ApiPropertyDescriptor> _properties;

        private ApiClassDescriptor(Type type, ApiContainer container, IDictionary<string, object> settings)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Apis = new ReadOnlyCollection<ApiDescriptor>(_apis = new List<ApiDescriptor>());
            Properties = new ReadOnlyCollection<ApiPropertyDescriptor>(_properties = new List<ApiPropertyDescriptor>());
            Name = type.Name;
            FullName = GetFullName(type);
        }

        private static string GetFullName(Type type)
            => type.IsNested ? $"{GetFullName(type.DeclaringType)}.{type.Name}" : type.Name;

        public Type Type { get; }
        /// <summary>
        /// 当前Api类的类名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 当前Api类的完整类名
        /// </summary>
        public string FullName { get; }
        public ICollection<ApiDescriptor> Apis { get; }
        public ICollection<ApiPropertyDescriptor> Properties { get; }

        public ApiContainer Container { get; }

        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="t"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        internal static ApiClassDescriptor Create(Type t, ApiContainer container)
        {
            var typeInfo = t.GetTypeInfo();
            if (typeInfo.IsAbstract || typeInfo.IsGenericTypeDefinition) //排除抽象类和泛型定义类型
            {
                return null;
            }

            var classAttrs = typeInfo.GetCustomAttributes<ApiClassAttribute>().ToArray();

            var settings = container.Provider.ParseSetting(classAttrs) ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var apiclass = new ApiClassDescriptor(t, container, settings);


            var propAttrs = typeInfo.DeclaredProperties
                                .Select(it => ApiPropertyDescriptor.Create(it, container, apiclass))
                                .Where(it => it != null);
            apiclass._properties.AddRange(propAttrs);

            var apis = typeInfo.DeclaredMethods
                                .Select(it => ApiDescriptor.Create(apiclass, it, container))
                                .Where(it => it != null);

            apiclass._apis.AddRange(apis);

            if (classAttrs.Length == 0 && apiclass._properties.Count == 0 && apiclass._apis.Count == 0)
            {
                return null;
            }

            return apiclass;
        }
    }
}
