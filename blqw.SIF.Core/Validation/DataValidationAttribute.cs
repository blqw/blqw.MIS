using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Validation
{
    /// <summary>
    /// 提供数据验证规则的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
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
        /// <param name="items"> 与要验证的值关联的键/值对的字典</param>
        /// <returns></returns>
        public virtual string GetErrorMessage(object value, IDictionary<string, object> items)
            => string.Format(ErrorMessageFormat, value ?? "<null>", value?.GetType() ?? (object)"`null`");

        /// <summary>
        /// 验证对象值是否有效
        /// </summary>
        /// <param name="arg">要验证的值</param>
        /// <param name="context"> Api调用上下文 </param>
        /// <returns></returns>
        public abstract bool IsValid(object arg, ApiCallContext context);

        
    }
}
