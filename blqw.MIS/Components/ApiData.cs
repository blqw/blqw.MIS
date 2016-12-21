using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.MIS
{
    /// <summary>
    /// Api数据
    /// </summary>
    public struct ApiData
    {
        /// <summary>
        /// 数据不存在
        /// </summary>
        public static ApiData NotFound = new ApiData();

        /// <summary>
        /// 初始化数据对象
        /// </summary>
        /// <param name="value"></param>
        public ApiData(object value)
        {
            Value = value;
            Error = null;
            Exists = true;
        }

        /// <summary>
        /// 初始化异常数据
        /// </summary>
        /// <param name="ex"></param>
        public ApiData(Exception ex)
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
