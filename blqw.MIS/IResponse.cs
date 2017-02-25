using System;
using System.IO;

namespace blqw.MIS
{
    /// <summary>
    /// 表示一个响应
    /// </summary>
    public interface IResponse : IFormattable
    {
        /// <summary>
        /// 获取真实响应对象
        /// </summary>
        /// <returns></returns>
        object GetActualResponse();
        /// <summary>
        /// 请求
        /// </summary>
        IRequest Request { get; }
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
