using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Owin.Exceptions
{
    public abstract class OwinException : Exception
    {
        protected OwinException()
        {
        }

        protected OwinException(string message) : base(message)
        {
        }

        protected OwinException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OwinException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
