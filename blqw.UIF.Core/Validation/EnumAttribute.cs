using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Validation
{
    /// <summary>
    /// 取值范围必须是枚举已定义的值
    /// </summary>
    public class EnumAttribute : DataValidationAttribute
    {
        protected EnumAttribute(string errorMessageFormat = "参数 {name} 值不在允许范围")
            : base(-102, errorMessageFormat ?? "参数 {name} 值不在允许范围")
        {
        }

        public override string GetDescription(Type type)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return $"可选值:{string.Join(", ", Enum.GetNames(type))}";
            }
            return null;
        }

        public override bool IsValid(object value, IDictionary<string, object> args)
            => value is Enum && Enum.IsDefined(value.GetType(), value);
    }
}
