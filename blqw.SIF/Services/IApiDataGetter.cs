using blqw.SIF.Descriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Services
{
    /// <summary>
    /// 用于获取api参数数据
    /// </summary>
    public interface IApiDataGetter
    {
        /// <summary>
        /// 尝试获取api参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="value">如果获取成功,返回参数值</param>
        /// <returns></returns>
        bool TryGetParameter(ApiParameterDescriptor parameter, out object value);
        /// <summary>
        /// 尝试获取api属性
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetProperty(string argName, out object value);
    }
}
