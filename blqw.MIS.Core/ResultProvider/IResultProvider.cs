using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// API结果提供程序接口
    /// </summary>
    public interface IResultProvider
    {
        /// <summary>
        /// API结果
        /// </summary>
        object Result { get; }
        /// <summary>
        /// API异常
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// API是否有错误
        /// </summary>
        bool IsError { get; }
    }
}
