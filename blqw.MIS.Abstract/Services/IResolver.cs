using System.Collections.Generic;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 提供解析请求的功能定义
    /// </summary>
    public interface IResolver: IService
    {
        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IDictionary<string, object> ParseArguments(IRequest request);
        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IDictionary<string, object> ParseProperties(IRequest request);
        /// <summary>
        /// 获取响应体
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponse GetResponse(IRequest request);
    }
}
