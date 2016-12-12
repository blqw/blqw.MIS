using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.UIF
{
    /// <summary>
    /// 表示API异常
    /// </summary>
    public class ApiException : Exception, IFormattable
    {
        public static ApiException ApiNotFound() => new ApiException(-404, "接口不存在");


        /// <summary>
        /// 参数错误 <paramref name="name"/> <paramref name="message"/>
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="message">消息</param>
        public static ApiException ArgumentError(string name, string message) => new ApiException(-100, $"参数错误 {name} {message}");
        /// <summary>
        /// 缺少参数 <paramref name="name"/>
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentMissing(string name) => new ApiException(-101, $"缺少参数 {name}");
        /// <summary>
        /// 参数 <paramref name="name"/> 值超过允许范围
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentOutRange(string name) => new ApiException(-102, $"参数 {name} 值超过允许范围");
        /// <summary>
        /// 参数 <paramref name="name"/> 格式错误
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentFormatError(string name) => new ApiException(-103, $"参数 {name} 格式错误");
        /// <summary>
        /// 参数 <paramref name="name"/> 类型错误
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentTypeError(string name) => new ApiException(-104, $"参数 {name} 类型错误");
        /// <summary>
        /// 参数 <paramref name="name"/> 非法
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentIllegality(string name) => new ApiException(-105, $"参数 {name} 非法");
        /// <summary>
        /// 参数 <paramref name="name"/> 无效
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentInvalid(string name) => new ApiException(-106, $"参数 {name} 无效");
        /// <summary>
        /// 参数 <paramref name="name"/> 值太长
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentTooLong(string name) => new ApiException(-107, $"参数 {name} 值太长");
        /// <summary>
        /// 参数 <paramref name="name"/> 值太大
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentTooBig(string name) => new ApiException(-108, $"参数 {name} 值太大");

        /// <summary>
        /// 参数 <paramref name="name"/> 不能为空
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns></returns>
        public static ApiException ArgumentEmpty(string name) => new ApiException(-109, $"参数 {name} 不能为空");


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
