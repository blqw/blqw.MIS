using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Exceptions
{
    /// <summary>
    /// Api设置异常
    /// </summary>
    public sealed class ApiSetupException : ApiException
    {
        /// <summary>
        /// 初始化异常
        /// </summary>
        /// <param name="message">异常说明</param>
        /// <param name="innerException">内部异常</param>
        public ApiSetupException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }
}
