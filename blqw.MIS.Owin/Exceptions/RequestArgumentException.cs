using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Owin.Exceptions
{
    /// <summary>
    /// 请求参数错误
    /// </summary>
    public abstract class RequestArgumentException : RequestException
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; }

        protected RequestArgumentException(int errorCode, string paramName, string message, Exception innerException = null)
            : base(200, string.Format(message ?? "参数{0}有误", paramName), innerException)
        {
            ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
            HResult = errorCode;
        }
    }
}
