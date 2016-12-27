using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using blqw.MIS.Descriptor;
using System.Collections.Specialized;
using System.IO;

namespace blqw.MIS.MVCAdapter
{
    class JsonGetter : IParameterGetter
    {
        private readonly NameValueCollection _query;
        private readonly HttpContext _context;
        private string _jsonString;
        private dynamic _entity;
        public JsonGetter(HttpContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            var buffer = ReadAll(context.Request.InputStream);
            _jsonString = context.Request.ContentEncoding.GetString(buffer);
            _query = context.Request.QueryString;
        }


        /// <summary>
        /// 字符串缓冲
        /// </summary>
        [ThreadStatic]
        private static byte[] _Buffer;
        /// <summary>
        /// 读取流中的所有字节
        /// </summary>
        /// <param name="stream"> </param>
        private static byte[] ReadAll(Stream stream)
        {
            var length = 1024;
            if (_Buffer == null)
            {
                _Buffer = new byte[length];
            }
            length = _Buffer.Length;
            var bytes = new List<byte>();
            int count;
            do
            {
                if ((count = stream.Read(_Buffer, 0, length)) == length)
                {
                    bytes.AddRange(_Buffer);
                }
                else
                {
                    bytes.AddRange(_Buffer.Take(count));
                }
            } while (count > 0);
            return bytes.ToArray();
        }
        
        public ApiData Get(ApiParameterDescriptor parameter)
        {
            var value = (object)_query.Get(parameter.Name); //优先从get中获取
            if (value != null)
            {
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

            if (parameter.Api.HasBody())
            {
                if (parameter.Name.Equals("body", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        value = Json.ToObject(parameter.ParameterType, _jsonString);
                        _jsonString = null;
                        return new ApiData(value);
                    }
                    catch (Exception ex)
                    {
                        return new ApiData(ex);
                    }
                }
                return ApiData.NotFound;
            }
            if (_entity == null)
            {
                _entity = Json.ToDynamic(_jsonString);
            }
            try
            {
                value = _entity[parameter.ParameterType];
                if (value == null)
                {
                    return ApiData.NotFound;
                }
                value = value.To(parameter.ParameterType);
                return new ApiData(value);
            }
            catch (Exception ex)
            {
                return new ApiData(ex);
            }
        }
    }
}
