using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.DataModification
{
    /// <summary>
    /// 提供数据更改规则的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class DataModificationAttribute : Attribute
    {
        /// <summary>
        /// 将一个值更改为类型相同的新值
        /// </summary>
        /// <param name="arg">待修改的值</param>
        /// <param name="context"> Api调用上下文 </param>
        /// <returns></returns>
        public virtual void Modifies(ref object arg, ApiCallContext context)
            => Modifies(ref arg);


        /// <summary>
        /// 将一个值更改为类型相同的新值
        /// </summary>
        /// <param name="arg">待修改的值</param>
        /// <returns></returns>
        public abstract void Modifies(ref object arg);


        /// <summary>
        /// 该值指示此实例是否等于指定的对象。
        /// </summary>
        /// <param name="attribute"> 要与此实例进行比较 <see cref="DataModificationAttribute"/>。</param>
        /// <returns></returns>
        public virtual bool Match(DataModificationAttribute attribute) => GetType() == attribute?.GetType();
    }
}
