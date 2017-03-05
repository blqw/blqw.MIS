using System.Collections.Generic;
using System.Reflection;

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
        IEnumerable<ApiArgument> ParseArguments(IRequest request);
        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IEnumerable<ApiProperty> ParseProperties(IRequest request);
        /// <summary>
        /// 创建Api所属类的实例
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object CreateApiClassInstance(IRequest request);
    }
}
