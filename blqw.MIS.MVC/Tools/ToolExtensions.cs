using blqw.MIS.MVC.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace blqw.MIS.MVC
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    internal static class ToolExtensions
    {
        [ThreadStatic]
        private static byte[] _buffer;

        /// <summary>
        /// 读取流中的所有字节
        /// </summary>
        /// <param name="stream"> </param>
        public static byte[] ReadAll(this Stream stream)
        {
            var length = 1024;
            if (_buffer == null)
            {
                _buffer = new byte[length];
            }
            length = _buffer.Length;
            var bytes = new List<byte>();
            int count;
            do
            {
                if ((count = stream.Read(_buffer, 0, length)) == length)
                {
                    bytes.AddRange(_buffer);
                }
                else
                {
                    bytes.AddRange(_buffer.Take(count));
                }
            } while (count > 0);
            return bytes.ToArray();
        }

        /// <summary>
        /// 获取 charset= 中描述的编码格式
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static Encoding GetEncoding(this string contentType)
        {
            if (contentType == null) return null;
            var start = contentType.IndexOf("charset=", StringComparison.OrdinalIgnoreCase);
            if (start < 0) return null;
            start += 8;
            var end = contentType.IndexOf(';');
            var charset = end < 0 ? contentType.Substring(start) : contentType.Substring(start, end - start);
            return string.IsNullOrWhiteSpace(charset) ? null : Encoding.GetEncoding(charset);
        }


        public static Request CastRequest(this IRequest request)
        {
            var req = (request ?? throw new ArgumentNullException(nameof(request))) as Request;
            return req ?? throw new ArgumentException($"{nameof(request)}类型只能是{typeof(Request).FullName}", nameof(request));
        }

        
    }
}
