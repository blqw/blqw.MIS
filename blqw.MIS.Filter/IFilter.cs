using System.Threading.Tasks;

namespace blqw.MIS.Filters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Api创建完成
        /// </summary>
        /// <param name="request"></param>
        Task OnApiCreated(IRequest request);
        /// <summary>
        /// Api参数解析完成
        /// </summary>
        /// <param name="request"></param>
        Task OnArgumentsParsed(IRequest request);
        /// <summary>
        /// Api属性解析完成
        /// </summary>
        /// <param name="request"></param>
        Task OnPropertiesParsed(IRequest request);
        /// <summary>
        /// 请求执行前触发
        /// </summary>
        /// <param name="request"></param>
        Task OnExecuting(IRequest request);
        /// <summary>
        /// 请求执行完毕触发
        /// </summary>
        /// <param name="request"></param>
        Task OnExecuted(IRequest request);
        /// <summary>
        /// 请求中出现错误时触发
        /// </summary>
        /// <param name="request"></param>
        Task OnError(IRequest request);
    }
}
