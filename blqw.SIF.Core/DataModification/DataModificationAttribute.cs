using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.DataModification
{
    /// <summary>
    /// 提供数据更改规则的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class DataModificationAttribute : Attribute
    {
        /// <summary>
        /// 将一个值更改为类型相同的新值
        /// </summary>
        /// <param name="arg">待修改的值</param>
        /// <param name="context"> Api调用上下文 </param>
        /// <returns></returns>
        public abstract object Modifies(object arg, ApiCallContext context);
    }
}
