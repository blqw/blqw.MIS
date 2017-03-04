using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 参数转换失败时引发异常
    /// </summary>
    public class ApiRequestPropertyCastException : ApiRequestArgumentException
    {
        public ApiRequestPropertyCastException(string paramName, string message = "属性{0}值有误", string detail = null, Exception innerException = null)
            : base(6500, paramName, message ?? "属性{0}值有误", innerException)
        {
            Detail = detail;
        }
    }
}
