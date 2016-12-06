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
        public FilterArgs(MethodInfo method, IDictionary<string, object> arguments)
        {
            Method = method;
            Arguments = arguments;
        }

        /// <summary>
        /// 当前api方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 获取或设置执行api方法时使用的参数
        /// </summary>
        public IDictionary<string, object> Arguments { get; }

        /// <summary>
        /// 执行api后得到的返回值
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 是否取消当前
        /// </summary>
        public bool Cancel { get; set; }
    }
}
