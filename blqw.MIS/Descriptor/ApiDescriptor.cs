using blqw.MIS.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace blqw.MIS.Descriptor
{
    /// <summary>
    /// 用于描述一个接口
    /// </summary>
    public class ApiDescriptor : IDescriptor
    {
        /// <summary>
        /// 初始化接口描述
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        public ApiDescriptor(MethodInfo method, ApiClassDescriptor apiClass, IDictionary<string, object> settings)
        {
            ApiClass = apiClass ?? throw new ArgumentNullException(nameof(apiClass));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Parameters = new ReadOnlyCollection<ApiParameterDescriptor>(method.GetParameters().Select(it => new ApiParameterDescriptor(it, apiClass)).ToList());
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Container = apiClass.Container;
            Name = method.Name;
            _invoker = CreateInvoker(method);

            var filters = new List<ApiFilterAttribute>();
            foreach (ApiFilterAttribute filter in method.GetCustomAttributes<ApiFilterAttribute>().FiltrateAttribute(Container.Provider, true)
                        .Union(method.DeclaringType.GetTypeInfo().GetCustomAttributes<ApiFilterAttribute>().FiltrateAttribute(Container.Provider, true))
                        .Union(Container.Filters.FiltrateAttribute(Container.Provider, true)))
            {
                if (filters.Any(a => a.Match(filter)) == false)
                {
                    filters.Add(filter);
                }
            }

            filters.Sort((a, b) => a.OrderNumber.CompareTo(b.OrderNumber));
            Filters = new ReadOnlyCollection<ApiFilterAttribute>(filters);
        }

        /// <summary>
        /// 接口方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 接口类
        /// </summary>
        public ApiClassDescriptor ApiClass { get; }

        /// <summary>
        /// 参数描述只读集合
        /// </summary>
        public IReadOnlyList<ApiParameterDescriptor> Parameters { get; }

        /// <summary>
        /// 属性描述只读集合
        /// </summary>
        public IReadOnlyList<ApiPropertyDescriptor> Properties => ApiClass.Properties;

        /// <summary>
        /// 过滤器只读集合
        /// </summary>
        public IReadOnlyList<ApiFilterAttribute> Filters { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// API容器
        /// </summary>
        public ApiContainer Container { get; }

        /// <summary>
        /// API设置
        /// </summary>
        public IDictionary<string, object> Settings { get; }

        /// <summary>
        /// 用于调用API方法的委托
        /// </summary>
        private Func<object, object[], object> _invoker;

        /// <summary>
        /// 创建API描述,如果方法不是API则返回null
        /// </summary>
        /// <param name="method">同于创建API的方法</param>
        /// <param name="apiclass">方法所在类的描述</param>
        /// <returns></returns>
        internal static ApiDescriptor Create(MethodInfo method, ApiClassDescriptor apiclass)
        {
            if (method.IsPublic && method.IsGenericMethodDefinition == false)
            {
                var attrs = method.GetCustomAttributes<ApiAttribute>();
                var settings = apiclass.Container.Provider.ParseSetting(attrs);
                if (settings == null)
                {
                    return null;
                }
                return new ApiDescriptor(method, apiclass, settings);
            }
            return null;
        }

        /// <summary>
        /// 调用API方法
        /// </summary>
        /// <param name="instance">API方法的实例</param>
        /// <param name="args">调用API方法所使用的参数</param>
        /// <returns></returns>
        internal object Invoke(object instance, object[] args)
        {
            if (args?.Length == Parameters.Count)
            {
                try
                {
                    return _invoker(instance, args);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
            return new ArgumentException("");
        }
        
        /// <summary>
        /// 创建指定方法的调用委托
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private Func<object, object[], object> CreateInvoker(MethodInfo method)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var args = Expression.Parameter(typeof(object[]), "args");

            var arguments = new List<Expression>();
            foreach (var p in method.GetParameters())
            {
                arguments.Add(Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(p.Position)), p.ParameterType));
            }

            var call = Expression.Call(method.IsStatic ? null : Expression.Convert(instance, method.DeclaringType), method, arguments.ToArray());

            if (method.ReturnType == typeof(void))
            {
                var lambda = Expression.Lambda<Action<object, object[]>>(call, instance, args);

                Action<object, object[]> execute = lambda.Compile();
                return (a, b) => { execute(a, b); return null; };
            }
            else
            {
                var ret = Expression.Convert(call, typeof(object));
                return Expression.Lambda<Func<object, object[], object>>(ret, instance, args).Compile();
            }
        }
    }
}
