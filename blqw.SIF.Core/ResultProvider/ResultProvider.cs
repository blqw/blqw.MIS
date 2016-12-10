using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    /// <summary>
    /// 一种简单的Api结果提供程序
    /// </summary>
    public sealed class ResultProvider : IResultProvider
    {
        public ResultProvider()
        {

        }

        public ResultProvider(object result)
        {
            Result = result;
        }

        /// <summary>
        /// Api异常
        /// </summary>
        public Exception Exception
        {
            get => Result as Exception;
            set => Result = value;
        }

        /// <summary>
        /// Api是否有错误
        /// </summary>
        public bool IsError => Result is Exception;

        /// <summary>
        /// Api结果
        /// </summary>
        public object Result { get; set; }

    }
}
