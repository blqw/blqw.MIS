using blqw.SIF.Descriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Services
{
    /// <summary>
    /// Api数据提供程序
    /// </summary>
    public interface IApiDataProvider
    {
        /// <summary>
        /// 尝试获取api参数
        /// </summary>
        /// <param name="parameter">参数描述</param>
        /// <param name="value">如果获取成功,返回参数值</param>
        /// <returns></returns>
        bool TryGetParameter(ApiParameterDescriptor parameter, out object value);
        /// <summary>
        /// 尝试获取api属性
        /// </summary>
        /// <param name="property">属性描述</param>
        /// <param name="value">如果获取成功,返回属性值</param>
        /// <returns></returns>
        bool TryGetProperty(ApiPropertyDescriptor property, out object value);
    }
}
