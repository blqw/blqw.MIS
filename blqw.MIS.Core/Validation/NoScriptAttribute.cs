using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace blqw.MIS.Validation
{
    /// <summary> 
    /// 验证字符串不能包含脚本字符
    /// </summary>
    public sealed class NoScriptAttribute : DataValidationAttribute
    {
        static readonly Regex HasScript = new Regex("(<script)|(<[^>]+(?<on>on[a-z]*\\s*)?=['\"\\s]*(?(on)|([a-z]*)script\\s*:))", RegexOptions.IgnoreCase);

        public NoScriptAttribute(string errorMessageFormat = "参数 {name} 中包含非法的脚本内容")
            : base(-105, errorMessageFormat ?? "参数 {name} 中包含非法的脚本内容")
        {
        }

        public override string GetDescription(Type type) => type == typeof(string) ? "不能包含脚本字符" : null;

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var str = value as string;
            if (str == null)
            {
                return true;
            }
            return HasScript.IsMatch(str) == false;
        }
    }
}
