using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.MVC.Services
{
    class JsonBodyParser : IBodyParser
    {
        public bool Match(string mineType)
            => mineType?.EndsWith("/json", StringComparison.OrdinalIgnoreCase) == true;

        public IEnumerable<ApiArgument> Parse(Request request)
        {
            var json = (request.Context.Request.ContentEncoding ?? Encoding.UTF8).GetString(request.Body);
            var obj = Json.ToObject(json) as IDictionary;
            var query = request.Context.Request.QueryString;
            var parameters = request.Api.Parameters;
            foreach (var p in request.Api.Parameters)
            {
                var value = obj?[p.Name] ?? query[p.Name];
                if (value == null)
                {
                    if (p.HasDefaultValue)
                    {
                        yield return new ApiArgument(p.Parameter, p.DefaultValue);
                    }
                    else if(request.Api.Parameters.Count == 1 && obj != null)
                    {
                        value = obj;
                    }
                    else
                    {
                        throw new ApiRequestArgumentNotFoundException(p.Name);
                    }
                }
                try
                {
                    value = value.ChangeType(p.ParameterType);
                }
                catch (Exception ex)
                {
                    var e = ex;
                    while (e.InnerException != null) e = e.InnerException;
                    throw new ApiRequestArgumentCastException(p.Name, detail: e.Message, innerException: ex);
                }
                yield return new ApiArgument(p.Parameter, 
                    value ?? (p.HasDefaultValue? p.DefaultValue : throw new ApiRequestArgumentNotFoundException(p.Name))
                    );
            }
        }
    }
}
