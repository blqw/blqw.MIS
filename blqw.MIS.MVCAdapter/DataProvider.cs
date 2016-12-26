using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using blqw.MIS.Descriptor;
using blqw.MIS.Services;
using blqw.MIS.Session;
using blqw;
using blqw.MIS.NetFramework45;
using System.Collections.Specialized;

namespace blqw.MIS.MVCAdapter
{
    internal class DataProvider : IApiDataProvider
    {
        private static Random _r = new Random();
        private readonly NameValueCollection _query;
        private readonly NameValueCollection _headers;
        private readonly NameValueCollection _body;

        private HttpContext _context;

        public DataProvider(HttpContext context)
        {
            _context = context;
            _body = context.Request.Form;
            _headers= context.Request.Headers;
            _query= context.Request.QueryString;
        }

        public object GetApiInstance(ApiDescriptor api) => null;

        public ApiData GetParameter(ApiParameterDescriptor parameter)
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

        private const string SESSION_KEY = "mis.mvc.sid";

        public ApiData GetProperty(ApiPropertyDescriptor property)
        {
            var value = (object)_headers[property.Name];
            if (value == null)
            {
                return ApiData.NotFound;
            }
            try
            {
                value = value.ChangeType(property.PropertyType);
                return new ApiData(value);
            }
            catch (Exception ex)
            {
                return new ApiData(ex);
            }
        }

        private static string CreateNewSesssionId() => $"{Guid.NewGuid():n}{_r.NextDouble()}";

        public ISession GetSession()
        {
            var sessionid = _context.Request.Cookies[SESSION_KEY]?.Value;
            var expires = 3600;
            return new MemorySession(sessionid, expires, CreateNewSesssionId);
        }

        internal void SaveSession(ISession session)
        {
            if (session?.IsNewSession == true)
            {
                _context.Response.Cookies.Add(new HttpCookie(SESSION_KEY, session.SessionId)
                {
                    HttpOnly = true,
                    Secure = _context.Request.IsSecureConnection,
                });
            }
        }
    }
}
