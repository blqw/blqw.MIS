using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using blqw.MIS.Descriptor;
using System.Collections.Specialized;

namespace blqw.MIS.MVCAdapter
{
    class FormGetter : IParameterGetter
    {
        private readonly HttpContext _context;
        private readonly NameValueCollection _query;
        private readonly NameValueCollection _body;

        public FormGetter(HttpContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _body = context.Request.Form;
            _query = context.Request.QueryString;
        }

        public ApiData Get(ApiParameterDescriptor parameter)
        {
            var value = (object)_body.Get(parameter.Name) ?? _query.Get(parameter.Name);
            if (value == null)
            {
                return ApiData.NotFound;
            }
            try
            {
                value = value.ChangeType(parameter.ParameterType);
                return new ApiData(value);
            }
            catch (Exception ex)
            {
                return new ApiData(ex);
            }
        }
    }
}
