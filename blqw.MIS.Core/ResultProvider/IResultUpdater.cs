using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 返回值更新程序
    /// </summary>
    public interface IResultUpdater : IResultProvider
    {
        /// <summary>
        /// 设置返回值
        /// </summary>
        /// <param name="result"> 被设置的值 </param>
        void SetResult(object result);

        /// <summary>
        /// 设置返回值异常
        /// </summary>
        /// <param name="exception"> 被设置的异常 </param>
        void SetException(Exception exception);
    }
}
