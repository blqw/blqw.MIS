using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace blqw.MIS.Descriptors
{
    /// <summary>
    /// 用于描述API
    /// </summary>
    public class ApiDescriptor : DescriptorBase
    {
        /// <summary>
        /// 初始化接口描述
        /// </summary>
        /// <param name="method">API指向的方法</param>
        /// <param name="apiClass">API方法所在的泪</param>
        public ApiDescriptor(MethodInfo method, ApiClassDescriptor apiClass)
            : base((apiClass ?? throw new ArgumentNullException(nameof(apiClass))).Container)
        {
            ApiClass = apiClass;
            CheckMethod(method, true);
            Method = method;
            Parameters = method.GetParameters().Select(it => new ApiParameterDescriptor(it, this)).AsReadOnly();
            Name = method.Name;
            _invoker = CreateInvoker(method);
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
        /// 用于调用API方法的委托
        /// </summary>
        private readonly Func<object, object[], object> _invoker;

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
        /// 创建API描述,如果方法不是API则返回null
        /// </summary>
        /// <param name="method">同于创建API的方法</param>
        /// <param name="apiclass">方法所在类的描述</param>
        /// <returns></returns>
        internal static ApiDescriptor Create(MethodInfo method, ApiClassDescriptor apiclass)
            => CheckMethod(method, false) ? new ApiDescriptor(method, apiclass) : null;

        /// <summary>
        /// 调用API方法
        /// </summary>
        /// <param name="instance">API方法的实例</param>
        /// <param name="args">调用API方法所使用的参数</param>
        /// <returns></returns>
        public object Invoke(object instance, object[] args)
        {
            try
            {
                return _invoker(instance, args);
            }
            catch (Exception ex)
            {
                ex = ex.ProcessException();
                return new InvalidOperationException("API调用失败:" + ex.Message, ex);
            }
        }


        /// <summary>
        /// 创建指定方法的调用委托
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static Func<object, object[], object> CreateInvoker(MethodInfo method)
        {
            var instance = Parameter(typeof(object), "instance");
            var args = Parameter(typeof(object[]), "args");
            var arguments = method.GetParameters()
                            .Select(p => Convert(ArrayIndex(args, Constant(p.Position)), p.ParameterType))
                            .ToArray<Expression>();
            var call = Call(method.IsStatic ? null : Convert(instance, method.DeclaringType), method, arguments);

            if (method.ReturnType == typeof(void))
            {
                var lambda = Lambda<Action<object, object[]>>(call, instance, args);
                var execute = lambda.Compile();
                return (a, b) => { execute(a, b); return ApiNoResultException.Instance; };
            }

            var ret = Convert(call, typeof(object));
            return Lambda<Func<object, object[], object>>(ret, instance, args).Compile();
        }

        public override string ToString()
            => $"[Api]:{Method}";
    }
}
