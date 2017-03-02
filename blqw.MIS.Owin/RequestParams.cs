using blqw.IOC;
using blqw.MIS.Descriptors;
using Microsoft.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace blqw.MIS.Owin
{
    class RequestParams
    {
        static RequestParams()
        {
            var t = typeof(MEF);
        }

        public IReadableStringCollection Query { get; }
        public IReadableStringCollection Body { get; }
        public IHeaderDictionary Header { get; }

        public RequestParams(IOwinContext context)
        {
            Query = context.Request.Query;
            Header = context.Request.Headers;
            Body = ParseBody(context);
        }

        /// <summary>
        /// 解析正文
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private IReadableStringCollection ParseBody(IOwinContext context)
        {
            var contentType = context.Request.ContentType;
            if (contentType == null || contentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                return context.Request.ReadFormAsync().Result;
            }
            if (contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                //throw new NotSupportedException("不支持的ContentType");
                return context.Request.ReadFormAsync().Result;
            }

            var format = GetFormat(context.Request.ContentType)?.ToLowerInvariant();
            if (format == "json")
            {
                var bytes = context.Request.Body.ReadAll();
                var encoding = GetEncoding(context.Request.ContentType) ?? Encoding.UTF8;
                var json = encoding.GetString(bytes);
                var obj = Json.ToObject(json);
                return new InnerBody(obj, t => Json.ToObject(t, json));
            }
            throw new NotSupportedException("不支持的ContentType");
        }

        private class InnerBody : IReadableStringCollection
        {
            private object _obj;
            private IDictionary _dict;
            private Func<Type, object> _converter;

            public InnerBody(object obj, Func<Type, object> converter)
            {
                _obj = obj;
                _dict = obj as IDictionary;
                _converter = converter;
            }

            public string this[string key] => Get(key);

            public string Get(string key)
            {
                if (_dict?.Contains(key) == true)
                {
                    return _dict[key]?.ToString();
                }
                return null;
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
            {
                if (_dict == null)
                {
                    if (_obj is IEnumerable || _obj is IEnumerator)
                    {
                        yield return new KeyValuePair<string, string[]>(null, _obj.To<string[]>());
                    }
                    else
                    {
                        yield return new KeyValuePair<string, string[]>(null, (string[])_converter(typeof(string[])));
                    }
                    yield break;
                }
                foreach (DictionaryEntry item in _dict)
                {
                    yield return new KeyValuePair<string, string[]>((string)item.Key, item.Value.To<string[]>());
                }
            }

            public IList<string> GetValues(string key)
            {
                if (_dict?.Contains(key) == true)
                {
                    return _dict[key].To<IList<string>>();
                }
                return null;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                if (_dict == null)
                {
                    yield return new KeyValuePair<string, object>(null, _obj);
                    yield break;
                }
                foreach (DictionaryEntry item in _dict)
                {
                    yield return new KeyValuePair<string, object>((string)item.Key, item.Value);
                }
            }
        }

        static char[] _contentTypeSeparator = new[] { ' ', ';' };

        private string GetFormat(string contentType)
        {
            if (contentType == null)
            {
                return null;
            }
            var i = contentType.IndexOf('/');
            if (i < 0)
            {
                return null;
            }
            i++;
            var e = contentType.IndexOfAny(_contentTypeSeparator);
            if (e < 0)
            {
                e = contentType.Length;
            }
            return contentType.Substring(i, e - i);
        }

        private Encoding GetEncoding(string contentType)
        {
            if (contentType == null)
            {
                return null;
            }
            var i = contentType.IndexOf("charset=");
            if (i < 0)
            {
                return null;
            }
            i++;
            var e = contentType.IndexOfAny(_contentTypeSeparator);
            if (e < 0)
            {
                e = contentType.Length;
            }
            try
            {
                return Encoding.GetEncoding(contentType.Substring(i, e - i));
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ParamValue GetParameter(ApiParameterDescriptor parameter)
        {
            var value = (object)Body.Get(parameter.Name) ?? Query.Get(parameter.Name);
            if (value == null)
            {
                return ParamValue.NotFound;
            }
            try
            {
                value = value.ChangeType(parameter.ParameterType);
                return new ParamValue(value);
            }
            catch (Exception ex)
            {
                return new ParamValue(ex);
            }
        }

        public ParamValue GetProperty(ApiPropertyDescriptor property)
        {
            var value = (object)Header.Get(property.Name);
            if (value == null)
            {
                return ParamValue.NotFound;
            }
            try
            {
                value = value.ChangeType(property.PropertyType);
                return new ParamValue(value);
            }
            catch (Exception ex)
            {
                return new ParamValue(ex);
            }
        }

    }
}
