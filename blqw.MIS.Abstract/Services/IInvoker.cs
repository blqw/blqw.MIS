namespace blqw.MIS.Services
{
    /// <summary>
    /// 提供指定API方法的功能定义
    /// </summary>
    public interface IInvoker : IService
    {
        /// <summary>
        /// 执行调用器得到返回值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object Execute(IRequest request);
    }
}
