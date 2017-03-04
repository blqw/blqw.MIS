using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 请求错误
    /// </summary>
    public class ApiRequestException : ApiException
    {
        public ApiRequestException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ResponseStatusCode = statusCode;
            HResult = statusCode;
        }

        public string Detail { get; protected set; }

        public int ResponseStatusCode { get; protected set; }
    }
}
