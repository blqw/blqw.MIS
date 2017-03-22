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
        /// API类初始化
        /// </summary>
        /// <param name="type">API类型对应的<see cref="Type"/></param>
        /// <param name="apis">API描述集合</param>
        /// <param name="properties">属性描述集合</param>
        public ApiClassDescriptor(Type type, IList<ApiDescriptor> apis, IList<ApiPropertyDescriptor> properties)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            CheckType(type.GetTypeInfo(), true);
            Apis = apis.AsReadOnly();
            Properties = properties.AsReadOnly();
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
    }
}
