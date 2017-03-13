using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using blqw.MIS.Descriptors;

namespace blqw.MIS
{
    /// <summary>
    /// 表示一个请求
    /// </summary>
    public interface IRequest : IFormattable
    {
        /// <summary>
        /// 当前请求的唯一标识
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 真实请求对象
        /// </summary>
        object ActualRequest { get; }
        /// <summary>
        /// 参数
        /// </summary>
        IReadOnlyList<ApiArgument> Arguments { get; }
        /// <summary>
        /// 属性
        /// </summary>
        IReadOnlyList<ApiProperty> Properties { get; }
        /// <summary>
        /// 当前请求的扩展信息
        /// </summary>
        IDictionary<string, object> Extends { get; }

        /// <summary>
        /// 获取或设置请求的返回值
        /// </summary>
        object Result { get; set; }
        /// <summary>
        /// API实例
        /// </summary>
        object Instance { get; }
        /// <summary>
        /// API方法
        /// </summary>
        ApiDescriptor Api { get; }
        /// <summary>
        /// 获取请求流
        /// </summary>
        /// <returns></returns>
        Stream GetStream();
        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        string ToString(string format);
    }
}
