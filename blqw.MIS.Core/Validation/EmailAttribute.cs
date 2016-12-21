using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace blqw.MIS.Validation
{
    /// <summary> 
    /// 必须是 Email 格式
    /// </summary>
    public class EmailAttribute : DataValidationAttribute
    {
        /// <summary> 
        /// 用于判断邮箱的正则表达式
        /// </summary>
        readonly static Regex _regexEmail = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

        protected EmailAttribute() 
            : base(-103, "参数 {name} 不是有效的 Email 格式")
        {

        }

        public override string GetDescription(Type type)
            => type == typeof(string) ? "必须是 Email 格式" : null;

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var str = value as string;
            if (str == null)
            {
                return false;
            }
            return _regexEmail.IsMatch(str);
        }
    }
}
