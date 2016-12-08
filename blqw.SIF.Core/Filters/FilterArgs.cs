using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Filters
{
    /// <summary>
    /// 过滤器参数
    /// </summary>
    public sealed class FilterArgs
    {
        /// <summary>
        /// 初始化过滤器参数
        /// </summary>
        /// <param name="method">当前api方法</param>
        /// <param name="arguments">执行api方法时使用的参数</param>
        public FilterArgs(ApiCallContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 当前api方法
        /// </summary>
        public ApiCallContext Context { get; }


        /// <summary>
        /// 执行api后得到的返回值
        /// </summary>
        public object Result
        {
            get => Context.Result;
            set => Context.Result = value;
        }

        /// <summary>
        /// 是否取消当前
        /// </summary>
        public bool Cancel { get; set; }
    }
}
