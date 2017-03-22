using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 默认的API类描述创建工厂
    /// </summary>
    public sealed class DefaultIApiClassDescriptorFactory : IApiClassDescriptorFactory
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static DefaultIApiClassDescriptorFactory Instance { get; } = new DefaultIApiClassDescriptorFactory();

        private DefaultIApiClassDescriptorFactory()
        {
            Data = new NameDictionary();
        }


        public void Dispose()
        {
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "默认类描述创建工厂";

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; }

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public bool RequireClone => false;

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public IService Clone() => new DefaultIApiClassDescriptorFactory();

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="type"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ApiClassDescriptor Create(Type type)
            => CreateApiClassDescriptor(type);

        #region Checked

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
        /// 检查方法合法性
        /// </summary>
        /// <param name="method">待检查的方法</param>
        /// <param name="throwIfError">是否抛出异常</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
        private static bool CheckMethod(MethodInfo method, bool throwIfError)
        {
            if (method == null)
            {
                return throwIfError ? throw new ArgumentNullException(nameof(method)) : false;
            }
            if (method.IsPublic == false)
            {
                return throwIfError ? throw new InvalidOperationException("受保护的方法不能用作API") : false;
            }
            if (method.IsAbstract)
            {
                return throwIfError ? throw new InvalidOperationException("抽象方法不能用作API") : false;
            }
            if (method.ContainsGenericParameters)
            {
                return throwIfError ? throw new InvalidOperationException("开放式泛型类型方法不能用作API") : false;
            }
            return true;
        }


        /// <summary>
        /// 检查属性合法性
        /// </summary>
        /// <param name="property">待检查的属性</param>
        /// <param name="throwIfError">是否抛出异常</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
        private static bool CheckProperty(PropertyInfo property, bool throwIfError)
        {
            if (property == null)
            {
                return throwIfError ? throw new ArgumentNullException(nameof(property)) : false;
            }
            var setter = property.SetMethod;
            if (setter == null)
            {
                return throwIfError ? throw new InvalidOperationException("未找到该属性的 set 访问器") : false;
            }
            if (setter.IsPublic == false)
            {
                return throwIfError ? throw new InvalidOperationException("受保护的属性不能用作API属性") : false;
            }
            if (setter.IsStatic)
            {
                return throwIfError ? throw new InvalidOperationException("静态属性不能用作API属性") : false;
            }
            if (setter.IsAbstract)
            {
                return throwIfError ? throw new InvalidOperationException("抽象属性不能用作API属性") : false;
            }
            if (setter.ContainsGenericParameters)
            {
                return throwIfError ? throw new InvalidOperationException("开放式泛型类型属性不能用作API属性") : false;
            }

            if (property.GetIndexParameters()?.Length > 0)
            {
                return throwIfError ? throw new InvalidOperationException("索引器不能声明为API属性") : false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 构建一个ApiClass描述,如果<paramref name="type"/>不是ApiClass则返回null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ApiClassDescriptor CreateApiClassDescriptor(Type type)
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
            var apis = new List<ApiDescriptor>();
            var properties = new List<ApiPropertyDescriptor>();
            var apiclass = new ApiClassDescriptor(type, apis, properties);

            apis.AddRange(methods
                .Select(it => CreateApiDescriptor(it, apiclass))
                .Where(it => it != null));
            if (apis.Count == 0)
            {
                return null;
            }

            properties.AddRange(typeInfo.DeclaredProperties
                .Where(it => it.IsDefined(typeof(ApiPropertyAttribute)))
                .Select(it => CreateApiPropertyDescriptor(it, apiclass))
                .Where(it => it != null));

            return apiclass;
        }


        /// <summary>
        /// 创建API描述,如果方法不是API则返回null
        /// </summary>
        /// <param name="method">同于创建API的方法</param>
        /// <param name="apiclass">方法所在类的描述</param>
        /// <returns></returns>
        private static ApiDescriptor CreateApiDescriptor(MethodInfo method, ApiClassDescriptor apiclass)
            => CheckMethod(method, false) ? new ApiDescriptor(method, apiclass) : null;


        /// <summary>
        /// 创建API属性描述,如果属性不合法,则返null
        /// </summary>
        /// <param name="property"></param>
        /// <param name="apiclass"></param>
        /// <returns></returns>
        private static ApiPropertyDescriptor CreateApiPropertyDescriptor(PropertyInfo property, ApiClassDescriptor apiclass)
            => CheckProperty(property, false) ? new ApiPropertyDescriptor(property, apiclass) : null;


    }
}
