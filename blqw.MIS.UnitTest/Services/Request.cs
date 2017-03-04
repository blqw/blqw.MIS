using blqw.MIS.Descriptors;
using blqw.MIS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace blqw.MIS.UnitTest
{
    /// <summary>
    /// Request
    /// </summary>
    public class Request : IRequestBase, IRequest, IRequestSetter
    {
        /// <summary>
        /// 当前请求的唯一标识
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("n");

        /// <summary>
        /// 真实请求对象
        /// </summary>
        public object ActualRequest { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public IReadOnlyList<ApiArgument> Arguments { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public IReadOnlyList<ApiProperty> Properties { get; set; }

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
        public object Instance { get; set; }

        /// <summary>
        /// API方法
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 获取请求流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream() => new MemoryStream();

        public override string ToString()
            => ActualRequest.ToString();

        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        public string ToString(string format)
            => ActualRequest.ToString();

        public string ToString(string format, IFormatProvider formatProvider)
            => ActualRequest.ToString();

        /// <summary>
        /// 接口描述
        /// </summary>
        public ApiDescriptor ApiDescriptor { get; set; }

        /// <summary>
        /// 请求实例
        /// </summary>
        IRequest IRequestSetter.Request => this;
    }
}
