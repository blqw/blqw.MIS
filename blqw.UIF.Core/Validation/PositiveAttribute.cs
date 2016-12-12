using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Validation
{
    /// <summary>
    /// 验证参数是否为正数
    /// </summary>
    public class PositiveAttribute : DataValidationAttribute
    {
        /// <summary>
        /// 是否允许值为0
        /// </summary>
        public bool AllowZero { get; }

        /// <summary>
        /// 初始化验证器
        /// </summary>
        /// <param name="allowZero">是否允许值为0,默认不允许</param>
        public PositiveAttribute(bool allowZero = false)
            : base(-102, $"参数 {{name}} 不能小于{(allowZero ? "" : "等于")}0")
        {
            AllowZero = allowZero;
        }


        public override string GetDescription(Type type) => $"不能小于{(AllowZero ? "" : "等于")}0";

        /// <summary>
        /// 验证对象值是否有效
        /// </summary>
        /// <param name="value">要验证的值</param>
        /// <param name="args"> 参数列表 </param>
        /// <returns></returns>
        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var comparable = value as IComparable;
            if (comparable == null)
            {
                return true;
            }
            var zero = Convert.ChangeType("0", value.GetType());
            var result = comparable.CompareTo(zero);
            return AllowZero ? result >= 0 : result > 0;
        }
    }
}
