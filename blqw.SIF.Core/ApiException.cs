using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF
{
    /// <summary>
    /// 表示API异常
    /// </summary>
    public class ApiException : Exception, IFormattable
    {
        public static readonly ApiException NotFound = new ApiException(-404, "接口不存在");

        public static readonly ApiException ArgumentCountError = new ApiException(-456, "参数个数错误");


        /// <summary>
        /// 初始化API异常
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ApiException(int errorCode, string message, Exception innerException = null)
            : base(message ?? innerException.Message, innerException)
        {
            if (errorCode == 0) throw new ArgumentOutOfRangeException(nameof(errorCode), "不能为零");
            HResult = errorCode;
        }

        /// <summary>
        /// API异常码
        /// </summary>
        public int ErrorCode => HResult;

        /// <summary>
        /// 返回异常信息字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => $"api error:{HResult}, {Message}" + (InnerException == null ? null : Environment.NewLine + InnerException.ToString());

        /// <summary>
        /// 根据格式化提供程序的定义格式化当前对象
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => (formatProvider?.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter)?.Format(format, this, formatProvider) ?? ToString();
    }
}
