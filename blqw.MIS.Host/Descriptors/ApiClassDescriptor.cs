using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 表示API所属类
    /// </summary>
    public class ApiClassDescriptor : DescriptorBase
    {
        /// <summary>
        /// API描述集合
        /// </summary>
        private readonly List<ApiDescriptor> _apis = new List<ApiDescriptor>();
        /// <summary>
        /// 属性描述集合
        /// </summary>
        private readonly List<ApiPropertyDescriptor> _properties = new List<ApiPropertyDescriptor>();

        /// <summary>
        /// API类初始化
        /// </summary>
        /// <param name="type">API类型对应的<see cref="Type"/></param>
        /// <param name="container">API容器</param>
        public ApiClassDescriptor(Type type, ApiContainer container)
            : base(container)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            CheckType(type.GetTypeInfo(), true);
            Apis = _apis.AsReadOnly();
            Properties = _properties.AsReadOnly();
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
        /// 检查类型的合法性
        /// </summary>
        /// <param name="type">待检查的类型</param>
        /// <param name="throwIfError">是否抛出异常</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
        private static bool CheckType(TypeInfo type, bool throwIfError)
        {
            if (type == null)
            {
                return throwIfError ? throw new ArgumentNullException(nameof(type)) : false;
            }
            if (type.IsClass == false)
            {
                return throwIfError ? throw new InvalidOperationException("只有Class类型可以用作API类") : false;
            }
            if (type.IsPublic == false)
            {
                return throwIfError ? throw new InvalidOperationException("受保护的类型不能用作API类") : false;
            }
            if (type.IsAbstract)
            {
                return throwIfError ? throw new InvalidOperationException("抽象类型不能用作API类") : false;
            }
            if (type.ContainsGenericParameters)
            {
                return throwIfError ? throw new InvalidOperationException("开放式泛型类型不能用作API类") : false;
            }
            if (type.IsPublic == false)
            {
                return throwIfError ? throw new InvalidOperationException("受保护的类型不能用作API类") : false;
            }
            return true;
        }

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="type"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <param name="container">Api容器</param>
        /// <returns></returns>
        internal static ApiClassDescriptor Create(Type type, ApiContainer container)
        {
            var typeInfo = type.GetTypeInfo();
            if (CheckType(typeInfo, false) == false)
            {
                return null;
            }

            var methods = typeInfo.DeclaredMethods.Where(m => m.IsDefined(typeof(ApiAttribute)));

            if (methods.Any() == false)
            {
                return null;
            }

            var apiclass = new ApiClassDescriptor(type, container);

            apiclass._apis.AddRange(methods
                .Select(it => ApiDescriptor.Create(it, apiclass))
                .Where(it => it != null));
            if (apiclass._apis.Count == 0)
            {
                return null;
            }

            apiclass._properties.AddRange(typeInfo.DeclaredProperties
                .Where(it => it.IsDefined(typeof(ApiPropertyAttribute)))
                .Select(it => ApiPropertyDescriptor.Create(it, apiclass))
                .Where(it => it != null));

            return apiclass;
        }
    }
}
