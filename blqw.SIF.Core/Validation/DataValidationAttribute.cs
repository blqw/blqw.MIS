using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Validation
{
    /// <summary>
    /// 提供数据验证规则的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class DataValidationAttribute : Attribute
    {
        /// <summary>
        /// 初始化验证基类
        /// </summary>
        /// <param name="errorCode">验证失败时的错误码</param>
        /// <param name="errorMessageFormat">验证失败时的异常信息,可以使用占位符,默认{0}表示验证失败的值,{1}表示验证失败的值类型</param>
        protected DataValidationAttribute(int errorCode, string errorMessageFormat)
        {
            ErrorCode = errorCode;
            ErrorMessageFormat = errorMessageFormat;
        }

        /// <summary>
        /// 验证失败时的错误码
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// 验证失败时的异常信息
        /// </summary>
        public string ErrorMessageFormat { get; }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="value">验证失败的值</param>
        /// <param name="context"> Api调用上下文 </param>
        /// <returns></returns>
        public virtual Exception GetException(object value, ApiCallContext context)
            => GetException(value, context?.Parameters);

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="value">验证失败的值</param>
        /// <param name="args"> 参数列表 </param>
        /// <returns></returns>
        public virtual Exception GetException(object value, IDictionary<string, object> args)
            => new ApiException(ErrorCode, string.Format(ErrorMessageFormat, value ?? "<null>", value?.GetType() ?? (object)"`null`"));

        /// <summary>
        /// 验证对象值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="context"> Api调用上下文 </param>
        /// <returns></returns>
        public virtual bool IsValid(object value, ApiCallContext context)
            => IsValid(value, context?.Parameters);


        /// <summary>
        /// 验证对象值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="args"> 参数列表 </param>
        /// <returns></returns>
        public abstract bool IsValid(object value, IDictionary<string, object> args);

        /// <summary>
        /// 该值指示此实例是否等于指定的对象。
        /// </summary>
        /// <param name="attribute"> 要与此实例进行比较 <see cref="ApiFilterAttribute"/>。</param>
        /// <returns></returns>
        public virtual bool Match(DataValidationAttribute attribute) => GetType() == attribute?.GetType();
    }
}
