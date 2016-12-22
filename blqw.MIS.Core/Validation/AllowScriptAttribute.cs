using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Validation
{
    /// <summary>
    /// 字符串允许包含脚本字符
    /// </summary>
    public class AllowScriptAttribute : DataValidationAttribute
    {
        public AllowScriptAttribute()
            : base(0, "")
        {

        }

        public override string GetDescription(Type type) => "允许包含脚本";

        public override bool IsValid(object value, IDictionary<string, object> args) => true;

        public override bool Match(DataValidationAttribute attribute)
            => base.Match(attribute) || attribute?.GetType() == typeof(NoScriptAttribute);

        protected override IEnumerable<Type> AllowTypes { get; } = new[] { typeof(string) };
    }
}
