using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Logging
{
    /// <summary>
    /// 日志项
    /// </summary>
    public struct LogItem
    {
        /// <summary>
        /// API上下文
        /// </summary>
        public ApiCallContext Context { get; set; }
        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel Level { get; set; }
        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 日志数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
