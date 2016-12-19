using blqw.UIF.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF
{
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 调试日志
        /// </summary>
        public static void Debug(this ApiCallContext context, string message, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message,
                Level = LogLevel.Debug,
                Title = title,
            });
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        public static void Debug(this ApiCallContext context, object data, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Data = data,
                Level = LogLevel.Debug,
                Title = title,
            });
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        public static void Information(this ApiCallContext context, string message, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message,
                Level = LogLevel.Information,
                Title = title,
            });
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        public static void Information(this ApiCallContext context, object data, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Data = data,
                Level = LogLevel.Information,
                Title = title,
            });
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        public static void Warning(this ApiCallContext context, string message, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message,
                Level = LogLevel.Warning,
                Title = title,
            });
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        public static void Warning(this ApiCallContext context, object data, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Data = data,
                Level = LogLevel.Warning,
                Title = title,
            });
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        public static void Error(this ApiCallContext context, string message, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message,
                Level = LogLevel.Error,
                Title = title,
            });
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        public static void Error(this ApiCallContext context, Exception exception, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Exception = exception,
                Level = LogLevel.Error,
                Title = title,
                Message = exception?.Message,
            });
        }

        /// <summary>
        /// 异常日志
        /// </summary>
        public static void Error(this ApiCallContext context, object data, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Data = data,
                Level = LogLevel.Error,
                Title = title,
            });
        }

        /// <summary>
        /// 致命错误日志
        /// </summary>
        public static void Critical(this ApiCallContext context, string message, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message,
                Level = LogLevel.Critical,
                Title = title,
            });
        }

        /// <summary>
        /// 致命错误日志
        /// </summary>
        public static void Critical(this ApiCallContext context, Exception exception, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Exception = exception,
                Level = LogLevel.Critical,
                Title = title,
                Message = exception?.Message,
            });
        }

        /// <summary>
        /// 致命错误日志
        /// </summary>
        public static void Critical(this ApiCallContext context, object data, string title = null)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Data = data,
                Level = LogLevel.Critical,
                Title = title,
            });
        }

        /// <summary>
        /// 自定义日志
        /// </summary>
        public static void Write(this ApiCallContext context, LogLevel level, string title, string message, object data, Exception exception)
        {
            context?.Logger?.Append(new LogItem
            {
                Context = context,
                Message = message ?? exception?.Message,
                Level = level,
                Title = title,
                Data = data,
                Exception = exception,
            });
        }
    }
}
