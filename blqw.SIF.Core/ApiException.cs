using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF
{
    /// <summary>
    /// 表示API异常
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(int errorCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            if (errorCode == 0) throw new ArgumentOutOfRangeException(nameof(errorCode), "不能为零");
            HResult = errorCode;
        }

        /// <summary>
        /// API异常码
        /// </summary>
        public int ErrorCode => HResult;
    }
}
