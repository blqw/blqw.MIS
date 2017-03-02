using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.MIS
{
    /// <summary>
    /// 参数值
    /// </summary>
    public struct ParamValue
    {
        /// <summary>
        /// 参数不存在
        /// </summary>
        public static ParamValue NotFound = new ParamValue();

        /// <summary>
        /// 初始化数据对象
        /// </summary>
        /// <param name="value"></param>
        public ParamValue(object value)
        {
            Value = value;
            Error = null;
            Exists = true;
        }

        /// <summary>
        /// 初始化异常数据
        /// </summary>
        /// <param name="ex"></param>
        public ParamValue(Exception ex)
        {
            Value = null;
            Error = ex ?? throw new ArgumentNullException(nameof(ex));
            Exists = true;
        }

        /// <summary>
        /// 数据值
        /// </summary>
        public object Value { get; }
        /// <summary>
        /// 获取数据时出现的错误
        /// </summary>
        public Exception Error { get; }
        /// <summary>
        /// 数据是否存在
        /// </summary>
        public bool Exists { get; }
    }
}
