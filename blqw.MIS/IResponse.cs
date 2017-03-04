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
        object GetActualResponse();
        /// <summary>
        /// 请求
        /// </summary>
        IRequest Request { get; }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        byte[] GetData();
        /// <summary>
        /// 返回请求的等效字符串
        /// </summary>
        /// <param name="format">格式化参数</param>
        /// <returns></returns>
        string ToString(string format);

        /// <summary>
        /// 是否有异常
        /// </summary>
        bool IsError { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        Exception Exception { get; }
    }
}
