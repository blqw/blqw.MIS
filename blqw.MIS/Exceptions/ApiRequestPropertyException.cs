using System;

namespace blqw.MIS
{
    /// <summary>
    /// 请求参数错误
    /// </summary>
    public abstract class ApiRequestPropertyException : ApiRequestException
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParamName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode">6000~6999</param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        protected ApiRequestPropertyException(int errorCode, string paramName, string message, Exception innerException = null)
            : base(200, string.Format(message ?? "属性{0}有误", paramName), innerException)
        {
            if (errorCode < 6000 || errorCode > 6999) throw new ArgumentOutOfRangeException(nameof(errorCode));
            ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
            HResult = errorCode;
        }
    }
}
