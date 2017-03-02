using System;
using System.IO;
using System.Threading.Tasks;

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
        Task<object> GetActualResponseAsync();
        /// <summary>
        /// 请求
        /// </summary>
        IRequest Request { get; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        Task<byte[]> GetDataAsync();
        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        string ToString(string format);
    }
}
