using blqw.MIS.Descriptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.MIS.Session;

namespace blqw.MIS.Services
{
    /// <summary>
    /// Api数据提供程序
    /// </summary>
    public interface IApiDataProvider
    {
        /// <summary>
        /// 获取api实例
        /// </summary>
        /// <param name="api">api描述</param>
        /// <returns></returns>
        object GetApiInstance(ApiDescriptor api);

        /// <summary>
        /// 获取api参数
        /// </summary>
        /// <param name="parameter">参数描述</param>
        /// <returns></returns>
        ApiData GetParameter(ApiParameterDescriptor parameter);
        /// <summary>
        /// 获取api属性
        /// </summary>
        /// <param name="property">属性描述</param>
        /// <returns></returns>
        ApiData GetProperty(ApiPropertyDescriptor property);

        /// <summary>
        /// 获取 Session
        /// </summary>
        /// <returns></returns>
        ISession GetSession();
    }
}
