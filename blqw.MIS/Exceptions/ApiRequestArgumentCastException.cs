﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 参数转换失败时引发异常
    /// </summary>
    public class ApiRequestArgumentCastException : ApiRequestArgumentException
    {
        public ApiRequestArgumentCastException(string paramName, string message = "参数[{0}]值有误", string detail = null, Exception innerException = null)
            : base(5500, paramName, message ?? "参数[{0}]值有误", innerException)
        {
            Detail = detail;
        }
    }
}
