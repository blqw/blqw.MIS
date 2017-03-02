using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    public abstract class ApiException : Exception
    {
        protected ApiException()
        {
        }

        protected ApiException(string message) : base(message)
        {
        }

        protected ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
