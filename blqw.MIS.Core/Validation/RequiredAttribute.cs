using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace blqw.MIS.Validation
{
    /// <summary>
    /// 非空验证
    /// </summary>
    public class RequiredAttribute : DataValidationAttribute
    {
        public RequiredAttribute(string errorMessageFormat = "参数 {name} 不能为空")
            : base(-109, errorMessageFormat ?? "参数 {name} 不能为空")
        {
        }

        public override string GetDescription(Type type) => "不能为空";

        /// <summary>
        /// 验证对象值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="args"> 参数列表 </param>
        /// <returns></returns>
        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            if (value == null)
            {
                return false;
            }
            if (value is string str)
            {
                return str.Length > 0;
            }
            if ((value as ICollection)?.Count > 0)
            {
                return true;
            }
            
            return ((value as IEnumerable)?.GetEnumerator() ?? (value as IEnumerator))?.MoveNext() ?? true;
        }

        public override bool IsAllowType(Type type)
            => type != null && type.GetTypeInfo().IsValueType == false;
    }
}