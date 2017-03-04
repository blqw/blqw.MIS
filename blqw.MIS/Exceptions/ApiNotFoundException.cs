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
    public class ApiNotFoundException : ApiException
    {
        public ApiNotFoundException(string message = "Api未找到", Exception innerException = null)
            : base(message, innerException)
        {
            ResponseStatusCode = 404;
            HResult = 404;
        }

        public string Detail { get; protected set; }

        public int ResponseStatusCode { get; protected set; }
    }
}
