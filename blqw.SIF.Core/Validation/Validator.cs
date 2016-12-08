using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF.Validation
{
    /// <summary>
    /// 验证器
    /// </summary>
    public static class Validator
    {

        private static readonly AttributeCache<MethodInfo, ParameterInfo, DataValidationAttribute> MethodCache = new AttributeCache<MethodInfo, ParameterInfo, DataValidationAttribute>();
        private static readonly AttributeCache<Type, PropertyInfo, DataValidationAttribute> TypeCache = new AttributeCache<Type, PropertyInfo, DataValidationAttribute>();

        /// <summary>
        /// 验证方法参数是否合法
        /// </summary>
        /// <param name="method">待验证参数的方法</param>
        /// <param name="args">需要验证的参数</param>
        /// <param name="lazy">懒惰模式,存在第一个不合法的参数立即返回,为false则返回所有不合法的验证结果</param>
        /// <returns>返回null验证通过</returns>
        public static Exception IsValid(MethodInfo method, IDictionary<string, object> args, bool lazy)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (args == null) throw new ArgumentNullException(nameof(args));
            List<Exception> errors = null;
            foreach (var p in MethodCache[method])
            {
                if (args.TryGetValue(p.Member.Name, out var value))
                {
                    if (p.Count > 0)
                    {
                        foreach (var valid in p)
                        {
                            if (valid.IsValid(value, args) == false)
                            {
                                if (lazy) return valid.GetException(value, args);
                                if (errors == null) errors = new List<Exception>();
                                errors.Add(valid.GetException(value, args));
                            }
                        }
                    }
                    var ex = IsValid(value, lazy);
                    if (ex != null)
                    {
                        if (lazy) return ex;
                        if (errors == null) errors = new List<Exception>();
                        errors.AddRange((ex as AggregateException).InnerExceptions);
                    }
                }
                else
                {
                    if (lazy) return ApiException.ArgumentMissing(p.Member.Name);
                    if (errors == null) errors = new List<Exception>();
                    errors.Add(ApiException.ArgumentMissing(p.Member.Name));
                }
            }
            if (errors == null)
            {
                return null;
            }
            return new AggregateException(errors);
        }

        /// <summary>
        /// 验证对象属性的值
        /// </summary>
        /// <param name="instance"> 待验证属性的对象 </param>
        /// <param name="lazy">懒惰模式,存在第一个不合法的参数立即返回,为false则返回所有不合法的验证结果</param>
        /// <returns></returns>
        public static Exception IsValid(object instance, bool lazy)
            => IsValid(instance, null, lazy);

        public static Exception IsValid(object instance, ApiCallContext context, bool lazy)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            List<Exception> errors = null;
            foreach (var p in TypeCache[instance.GetType()])
            {
                var value = p.Getter(instance);
                foreach (var valid in p)
                {
                    if (valid.IsValid(value, context) == false)
                    {
                        if (lazy) return valid.GetException(value, context?.Parameters);
                        if (errors == null) errors = new List<Exception>();
                        errors.Add(valid.GetException(value, context?.Parameters));
                    }
                }
            }
            if (errors == null)
            {
                return null;
            }
            return new AggregateException(errors);
        }
    }
}
