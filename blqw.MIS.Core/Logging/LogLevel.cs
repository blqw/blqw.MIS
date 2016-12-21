using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Logging
{
    /// <summary>
    /// 日志等级
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 全部
        /// </summary>
        All = -1,
        /// <summary>
        /// 调试跟踪。
        /// </summary>
        Debug = 0b0_0001,
        /// <summary>
        /// 信息性消息。
        /// </summary>
        Information = 0b0_0010,
        /// <summary>
        /// 非严重问题。
        /// </summary>
        Warning = 0b0_0100,
        /// <summary>
        /// 可恢复的错误。
        /// </summary>
        Error = 0b0_1000,
        /// <summary>
        /// 致命错误或应用程序崩溃。
        /// </summary>
        Critical = 0b1_0000,
        /// <summary>
        /// 包含 <see cref="Debug"/> | <see cref="Information"/>
        /// </summary>
        Informations = 0b0_0011,
        /// <summary>
        /// 包含 <see cref="Debug"/> | <see cref="Information"/> | <see cref="Warning"/>
        /// </summary>
        Warnings = 0b0_0111,
        /// <summary>
        /// 包含 <see cref="Debug"/> | <see cref="Information"/> | <see cref="Warning"/> | <see cref="Error"/>
        /// </summary>
        Errors = 0b0_1111,
        /// <summary>
        /// 包含 <see cref="Debug"/> | <see cref="Information"/> | <see cref="Warning"/> | <see cref="Error"/>
        /// </summary>
        Criticals = 0b1_1111,
    }
}
