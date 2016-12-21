using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Logging
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// 日志开关
        /// </summary>
        LogLevel Switch { get; }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log"> 日志项 </param>
        void Append(LogItem log);

        /// <summary>
        /// 刷新日志到存储
        /// </summary>
        void Flush();
    }
}
