using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace blqw.MIS.Validation
{
    /// <summary> 验证值是否符合正则表达式的标准
    /// </summary>
    public sealed class RegexAttribute : DataValidationAttribute
    {
        /// <summary> 验证用的正则表达式
        /// </summary>
        private readonly Regex _regex;

        private readonly string _pattern;

        /// <summary> 用正则表达式和表达式设置初始化特性
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <param name="options">表达式设置</param>
        public RegexAttribute(string pattern, RegexOptions options = RegexOptions.None)
            : base(-103, "参数 {name} 格式不正确")
        {
            _pattern = pattern;
            _regex = new Regex(pattern, options, TimeSpan.FromSeconds(3));
        }
        
        public override string GetDescription(Type type) => $"值必须通过正则表达式({_pattern})的验证";

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var str = value as string;
            return str == null || _regex.IsMatch(str);
        }
        protected override IEnumerable<Type> AllowTypes { get; } = new[] { typeof(string) };
    }
}
