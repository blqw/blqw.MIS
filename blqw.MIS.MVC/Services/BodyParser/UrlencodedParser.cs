using blqw.MIS.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace blqw.MIS.MVC.Services
{
    class UrlencodedParser : IBodyParser
    {
        public bool Match(string mineType)
            => string.Equals("application/x-www-form-urlencoded", mineType, StringComparison.OrdinalIgnoreCase);

        public IEnumerable<ApiArgument> Parse(Request request)
        {
            var param = request.Context.Request;
            foreach (var p in request.Api.Parameters)
            {
                var value = (object)param.Form[p.Name] ?? param.QueryString[p.Name];
                if (value == null && p.IsEntity)
                {
                    value = ParseEntity(p.ParameterType, p.Properties, param, p.Name + ".", false);
                }
                if (value == null)
                {
                    if (p.HasDefaultValue)
                    {
                        yield return new ApiArgument(p.Parameter, p.DefaultValue);
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
                yield return new ApiArgument(p.Parameter, value);
            }
        }


        private object ParseEntity(Type entityType, IEnumerable<ApiPropertyDescriptor> properties, HttpRequest param, string prefix, bool fullNameOnly)
        {
            var entity = Activator.CreateInstance(entityType);
            var hasValue = false;
            foreach (var p in properties)
            {
                var fullname = prefix + p.Name;
                var value = (object)param.Form[fullname] ?? param.QueryString[fullname];
                if (value == null && fullNameOnly == false)
                {
                    value = param.Form[p.Name] ?? param.QueryString[p.Name];
                }
                if (value == null && p.IsEntity)
                {
                    value = ParseEntity(p.PropertyType, p.Properties, param, fullname + ".", true);
                }
                if (value == null && p.HasDefaultValue)
                {
                    value = p.DefaultValue;
                }
                if (value != null)
                {
                    hasValue = true;
                    try
                    {
                        value = value.ChangeType(p.PropertyType);
                        p.SetValue(entity, value);
                    }
                    catch (Exception e)
                    {
                        throw new ApiRequestArgumentCastException(fullname, detail: e.Message, innerException: e);
                    }
                }

            }
            return hasValue ? entity : null;
        }

    }
}
