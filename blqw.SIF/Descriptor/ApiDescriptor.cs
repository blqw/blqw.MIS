using blqw.SIF.Services;
using blqw.SIF.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using blqw.SIF.Filters;

namespace blqw.SIF.Descriptor
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
        public ApiDescriptor(ApiClassDescriptor apiClass, MethodInfo method, ApiContainer container, IDictionary<string, object> settings)
        {
            ApiClass = apiClass ?? throw new ArgumentNullException(nameof(apiClass));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Parameters = new ReadOnlyCollection<ApiParameterDescriptor>(method.GetParameters().Select(it => new ApiParameterDescriptor(apiClass, it, container)).ToList());
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Name = method.Name;
            _invoker = CreateInvoker(method);

            var filters = new List<ApiFilterAttribute>();
            foreach (ApiFilterAttribute filter in method.GetCustomAttributes<ApiFilterAttribute>().FiltrateAttribute(container.Services, true)
                        .Union(method.DeclaringType.GetTypeInfo().GetCustomAttributes<ApiFilterAttribute>().FiltrateAttribute(container.Services, true))
                        .Union(container.Filters.FiltrateAttribute(container.Services, true)))
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
        /// 参数描述集合
        /// </summary>
        public ICollection<ApiParameterDescriptor> Parameters { get; }

        /// <summary>
        /// 属性描述集合
        /// </summary>
        public ICollection<ApiPropertyDescriptor> Properties => ApiClass.Properties;

        public ICollection<ApiFilterAttribute> Filters { get; }

        public string Name { get; }

        public ApiContainer Container { get; }

        public IDictionary<string, object> Settings { get; }

        private Func<object, object[], object> _invoker;
        internal static ApiDescriptor Create(ApiClassDescriptor apiclass, MethodInfo m, ApiContainer container)
        {
            if (m.IsPublic && m.IsGenericMethodDefinition == false)
            {
                var attrs = m.GetCustomAttributes<ApiAttribute>();
                var settings = container.Services.ParseSetting(attrs);
                if (settings == null)
                {
                    return null;
                }
                return new ApiDescriptor(apiclass, m, container, settings);
            }
            return null;
        }

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
