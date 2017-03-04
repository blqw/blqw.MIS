using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 请求参数错误
    /// </summary>
    public abstract class ApiRequestArgumentException : ApiRequestException
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; }

        protected ApiRequestArgumentException(int errorCode, string paramName, string message, Exception innerException = null)
            : base(200, string.Format(message ?? "参数[{0}]有误", paramName), innerException)
        {
            if (errorCode < 5000 || errorCode > 5999) throw new ArgumentOutOfRangeException(nameof(errorCode));
            ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
            HResult = errorCode;
        }
    }
}
