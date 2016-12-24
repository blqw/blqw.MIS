using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.MIS.Services;
using System.Reflection;
using System.Collections.ObjectModel;

namespace blqw.MIS.Descriptor
{
    /// <summary>
    /// 表示一个API类
    /// </summary>
    public class ApiClassDescriptor : IDescriptor
    {
        /// <summary>
        /// API描述集合
        /// </summary>
        private readonly List<ApiDescriptor> _apis;
        /// <summary>
        /// 属性描述集合
        /// </summary>
        private readonly List<ApiPropertyDescriptor> _properties;

        /// <summary>
        /// API类初始化
        /// </summary>
        /// <param name="type">API类型对应的<see cref="Type"/></param>
        /// <param name="container">API容器</param>
        /// <param name="settings">API设置</param>
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

        /// <summary>
        /// 获取类的完整类型名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static string GetFullName(Type type)
            => type.IsNested ? $"{GetFullName(type.DeclaringType)}.{type.Name}" : type.Name;

        /// <summary>
        /// 当前API类对应的 <see cref="Type"/> 对象
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// 当前API类的类名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 当前API类的完整类名
        /// </summary>
        public string FullName { get; }
        /// <summary>
        /// API描述只读集合
        /// </summary>
        public IReadOnlyList<ApiDescriptor> Apis { get; }
        /// <summary>
        /// 属性描述只读集合
        /// </summary>
        public IReadOnlyList<ApiPropertyDescriptor> Properties { get; }
        /// <summary>
        /// API容器
        /// </summary>
        public ApiContainer Container { get; }
        /// <summary>
        /// API类设置
        /// </summary>
        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="type"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ApiClassDescriptor Create(Type type, ApiContainer container)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass || typeInfo.IsAbstract || typeInfo.IsGenericTypeDefinition) //排除抽象类和泛型定义类型
            {
                return null;
            }

            if (typeInfo.DeclaredMethods.Any(m => m.IsDefined(typeof(ApiAttribute))) == false)
            {
                return null;
            }

            var classAttrs = typeInfo.GetCustomAttributes<ApiClassAttribute>().ToArray();

            var settings = container.Provider.ParseSetting(classAttrs) ?? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            var apiclass = new ApiClassDescriptor(type, container, settings);

            var propAttrs = typeInfo.DeclaredProperties
                                .Select(it => ApiPropertyDescriptor.Create(it, apiclass))
                                .Where(it => it != null);
            apiclass._properties.AddRange(propAttrs);

            var apis = typeInfo.DeclaredMethods
                                .Select(it => ApiDescriptor.Create(it, apiclass))
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
