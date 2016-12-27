using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using blqw.MIS.Descriptor;
using System.Web;
using System.Collections.Specialized;
using System.IO;

namespace blqw.MIS.MVCAdapter
{
    internal class MultipartGetter : FormGetter
    {
        private readonly HttpFileCollection _files;

        public MultipartGetter(HttpContext context)
            : base(context)
        {
            _files = context.Request.Files;
        }

        public override ApiData Get(ApiParameterDescriptor parameter)
        {
            var data = base.Get(parameter);
            if (data.Exists) return data;
            var file = _files[parameter.Name];
            if (file == null) return ApiData.NotFound;
            if (parameter.ParameterType == typeof(byte[]))
            {
                return new ApiData(ReadAll(file.InputStream));
            }
            if (parameter.ParameterType == typeof(Stream))
            {
                return new ApiData(file.InputStream);
            }
            if (parameter.ParameterType == typeof(HttpPostedFile))
            {
                return new ApiData(file);
            }

            return new ApiData(ReadAll(file.InputStream));
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
    }
}
