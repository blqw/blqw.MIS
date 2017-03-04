using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 当前请求无返回值
    /// </summary>
    public class ApiNoResultException : ApiRequestException
    {
        public static ApiNoResultException Instance { get; } = new ApiNoResultException();
        public ApiNoResultException(string message = "无返回值", Exception innerException = null)
            : base(205, message, innerException)
        {
        }
    }
}
