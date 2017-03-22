using System.Collections.Generic;
using blqw.MIS.Descriptors;
using blqw.MIS.Services;

namespace blqw.MIS.MVC.Services
{
    internal class RequestSetter : IRequestSetter
    {
        private readonly Request _request;

        public RequestSetter(Request request)
        {
            _request = request;
        }
        public IReadOnlyList<ApiArgument> Arguments { set => _request.Arguments = value; }
        public IReadOnlyList<ApiProperty> Properties { set => _request.Properties = value; }
        public object Result { set => _request.Result = value; }
        public object Instance { set => _request.Instance = value; }
        public ApiDescriptor ApiDescriptor { set => _request.Api = value; }

        public IRequest Request => _request;
    }
}
