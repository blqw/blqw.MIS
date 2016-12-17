using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF
{
    /// <summary>
    /// Api结果提供程序接口
    /// </summary>
    public interface IResultProvider
    {
        /// <summary>
        /// Api结果
        /// </summary>
        object Result { get; }
        /// <summary>
        /// Api异常
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// Api是否有错误
        /// </summary>
        bool IsError { get; }
    }
}
