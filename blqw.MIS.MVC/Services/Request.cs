using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using blqw.MIS.Descriptors;
using blqw.MIS.Services;

namespace blqw.MIS.MVC.Services
{
    /// <summary>
    /// 基于<seealso cref="HttpContext"/>实现的 IRequest
    /// </summary>
    public class Request : IRequestBase, IRequest
    {

        public Request(HttpContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Id = Guid.NewGuid().ToString("n");
        }

        /// <summary>
        /// 当前请求的唯一标识
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 真实请求对象
        /// </summary>
        public HttpRequest ActualRequest => Context.Request;

        public HttpContext Context { get; }

        private byte[] _body;
        public byte[] Body => _body ?? (_body = Context.Request.BinaryRead(Context.Request.ContentLength));

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
            var request = Context.Request;
            switch (format)
            {
                case "all":
                    var buffer = StringBuilderPool.GetOut();
                    buffer.Append(request.HttpMethod);
                    buffer.Append(" ");
                    buffer.Append(request.Url);
                    buffer.Append(" ");
                    buffer.AppendLine(request.RequestType);
                    var headers = request.Headers;
                    foreach (var key in headers.AllKeys)
                    {
                        foreach (var value in headers.GetValues(key))
                        {
                            buffer.Append(key);
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
