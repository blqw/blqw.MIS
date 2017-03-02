using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;
using blqw.MIS.Services;
using Microsoft.Owin;

namespace blqw.MIS.Owin.Services
{
    /// <summary>
    /// 基于<seealso cref="OwinRequest"/>实现的 IRequest
    /// </summary>
    public class Request : IRequestBase, IRequest
    {

        public Request(IOwinContext owin)
        {
            OwinContext = owin ?? throw new ArgumentNullException(nameof(owin));
            Id = "Owin-" + Guid.NewGuid().ToString("n");
        }

        /// <summary>
        /// 当前请求的唯一标识
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 真实请求对象
        /// </summary>
        public IOwinRequest ActualRequest => OwinContext.Request;

        public IOwinContext OwinContext { get; }

        private byte[] _body;
        public byte[] Body => _body ?? (_body = OwinContext.Request.Body.ReadAll());

        /// <summary>
        /// 参数
        /// </summary>
        public IReadOnlyList<ApiArgument> Arguments { get; internal set; }

        /// <summary>
        /// 属性
        /// </summary>
        public IReadOnlyList<ApiProperty> Properties { get; internal set; }

        private IDictionary<string, object> _extends;

        /// <summary>
        /// 当前请求的扩展信息
        /// </summary>
        public IDictionary<string, object> Extends => _extends ?? (_extends = new NameDictionary());

        /// <summary>
        /// 获取或设置请求的返回值
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// API实例
        /// </summary>
        public object Instance { get; internal set; }

        /// <summary>
        /// API方法
        /// </summary>
        public MethodInfo Method => ApiDescriptor?.Method;

        /// <summary>
        /// 获取请求流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream() => new MemoryStream(Body);

        public override string ToString() => ToString(null);

        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        public string ToString(string format)
            => ToString(format, null);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            var request = OwinContext.Request;
            switch (format)
            {
                case "all":
                    var buffer = StringBuilderPool.GetOut();
                    buffer.Append(request.Method);
                    buffer.Append(" ");
                    buffer.Append(request.Uri);
                    buffer.Append(" ");
                    buffer.AppendLine(request.Accept);
                    foreach (var header in request.Headers)
                    {
                        foreach (var value in header.Value)
                        {
                            buffer.Append(header.Key);
                            buffer.Append(":");
                            buffer.AppendLine(value);
                        }
                    }
                    buffer.AppendLine();
                    buffer.Append((request.ContentType.GetEncoding() ?? Encoding.Default).GetString(Body));
                    return StringBuilderPool.Return(buffer);
                case "body":
                case null:
                default:
                    return (request.ContentType.GetEncoding() ?? Encoding.Default).GetString(Body);
            }

        }

        /// <summary>
        /// 接口描述
        /// </summary>
        public ApiDescriptor ApiDescriptor { get; internal set; }
        

        object IRequest.ActualRequest => ActualRequest;
    }
}
