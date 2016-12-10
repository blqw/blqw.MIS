using System;
using System.Collections;
using System.Collections.Generic;

namespace blqw.SIF.Validation
{
    /// <summary>
    /// 非空验证
    /// </summary>
    public class RequiredAttribute : DataValidationAttribute
    {
        protected RequiredAttribute(string errorMessageFormat = "参数 {name} 不能为空")
            : base(-109, errorMessageFormat ?? "参数 {name} 不能为空")
        {
        }

        public override string GetDescription(Type type) => "不能为空";

        /// <summary> 判断值是否非空
        /// </summary>
        /// <param name="obj">待判断的值</param>
        /// <returns></returns>
        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            if (value == null)
            {
                return false;
            }
            var str = value as string;
            if (str != null)
            {
                return str.Length > 0;
            }
            if ((value as ICollection)?.Count > 0)
            {
                return true;
            }
            return ((value as IEnumerable)?.GetEnumerator() ?? (value as IEnumerator))?.MoveNext() ?? true;
        }
    }
}