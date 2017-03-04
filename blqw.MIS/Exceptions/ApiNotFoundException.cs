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
    public class ApiNotFoundException : ApiRequestException
    {
        public ApiNotFoundException(string message = "Api未找到", Exception innerException = null)
            : base(404, message, innerException)
        {
        }
    }
}
