﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Owin.Exceptions
{
    /// <summary>
    /// 参数没找到时引发的异常
    /// </summary>
    public class RequestArgumentNotFoundException : RequestArgumentException
    {
        public RequestArgumentNotFoundException(string paramName, string message = "参数{0}不存在", Exception innerException = null)
            : base(5404, paramName, message ?? "参数{0}不存在", innerException)
        {

        }
    }
}
