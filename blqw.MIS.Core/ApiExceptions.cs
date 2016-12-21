using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    public partial class ApiException
    {
        public static ApiException ApiNotFound { get; } = new ApiException(-404, "接口不存在");

        public static ApiException NotSupportedSession { get; } = new ApiException(-5, "当前Host或Adapter不支持Session");


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


    }
}
