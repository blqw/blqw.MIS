using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 一种简单的API结果提供程序
    /// </summary>
    public sealed class ResultProvider : IResultUpdater
    {
        public ResultProvider()
        {

        }

        public ResultProvider(object result)
        {
            Result = result;
        }

        /// <summary>
        /// API异常
        /// </summary>
        public Exception Exception => Result as Exception;

        /// <summary>
        /// API是否有错误
        /// </summary>
        public bool IsError => Result is Exception;

        /// <summary>
        /// API结果
        /// </summary>
        public object Result { get; set; }

        public void SetResult(object result) => Result = result;

        public void SetException(Exception exception) => Result = exception;
    }
}
