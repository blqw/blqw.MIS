using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 提供选择API的功能定义
    /// </summary>
    public interface ISelector : IService
    {
        /// <summary>
        /// 返回一个接口描述
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        ApiDescriptor FindApi(IRequest request);
    }
}
