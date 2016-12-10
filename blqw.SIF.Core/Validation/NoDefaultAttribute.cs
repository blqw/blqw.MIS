using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Validation
{
    /// <summary> 
    /// 不能为空,不能为默认值
    /// </summary>
    public class NoDefaultAttribute : RequiredAttribute
    {
        protected NoDefaultAttribute(string errorMessageFormat = "参数 {name} 不能为 default")
            : base(errorMessageFormat ?? "参数 {name} 不能为 default")
        {
        }

        public override string GetDescription(Type type) => "不能为默认值";

        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var v = value as ValueType;
            if (v != null)
            {
                var defaultValue = Activator.CreateInstance(value.GetType());
                return v.Equals(defaultValue) == false;
            }
            return base.IsValid(value, args);
        }
    }
}
