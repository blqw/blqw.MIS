using System.Threading.Tasks;

namespace blqw.MIS.Filters
{
    /// <summary>
    /// 过滤器
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 请求参数准备中触发
        /// </summary>
        /// <param name="request"></param>
        void OnReadying(IRequest request);
        /// <summary>
        /// 请求参数已准备完成时触发
        /// </summary>
        /// <param name="request"></param>
        void OnReadied(IRequest request);
        /// <summary>
        /// 请求执行前触发
        /// </summary>
        /// <param name="request"></param>
        void OnExecuting(IRequest request);
        /// <summary>
        /// 请求执行完毕触发
        /// </summary>
        /// <param name="request"></param>
        void OnExecuted(IRequest request);
        /// <summary>
        /// 请求中出现错误时触发
        /// </summary>
        /// <param name="request"></param>
        void OnError(IRequest request);


        /// <summary>
        /// 请求参数准备中触发
        /// </summary>
        /// <param name="request"></param>
        Task OnReadyingAsync(IRequest request);
        /// <summary>
        /// 请求参数已准备完成时触发
        /// </summary>
        /// <param name="request"></param>
        Task OnReadiedAsync(IRequest request);
        /// <summary>
        /// 请求执行前触发
        /// </summary>
        /// <param name="request"></param>
        Task OnExecutingAsync(IRequest request);
        /// <summary>
        /// 请求执行完毕触发
        /// </summary>
        /// <param name="request"></param>
        Task OnExecutedAsync(IRequest request);
        /// <summary>
        /// 请求中出现错误时触发
        /// </summary>
        /// <param name="request"></param>
        Task OnErrorAsync(IRequest request);
    }
}
