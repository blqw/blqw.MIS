using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Services
{
    /// <summary>
    /// 类型转换接口
    /// </summary>
    public interface IConverter : IService
    {
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="value"> 待转换类型的值 </param>
        /// <param name="conversionType">  要返回的对象的类型。 </param>
        /// <returns></returns>
        /// <remarks>此接口不应抛出异常</remarks>
        ConvertResult ChangeType(object value, Type conversionType);
    }

    /// <summary>
    /// 类型转换结果
    /// </summary>
    public struct ConvertResult
    {
        /// <summary>
        /// 初始化结果
        /// </summary>
        /// <param name="value">转换后的值</param>
        public ConvertResult(object value)
        {
            Value = value;
            Succeed = true;
            Error = null;
        }

        /// <summary>
        /// 初始化转换异常
        /// </summary>
        /// <param name="error">转换中的异常</param>
        public ConvertResult(Exception error)
        {
            Value = null;
            Succeed = false;
            Error = error;
        }
        /// <summary>
        /// 转换结果
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 是否转换成功
        /// </summary>
        public bool Succeed { get; }
        /// <summary>
        /// 转换中的异常信息
        /// </summary>
        public Exception Error { get; }
    }
}
