using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF
{
    public class ApiException : Exception
    {
        public ApiException(int errorCode, string message,Exception innerException = null)
            : base(message,innerException)
        {
            HResult = errorCode;
            ErrorCode = errorCode;
            Data["ApiException"] = "ApiException";
        }

        public int ErrorCode { get; }
    }
}
