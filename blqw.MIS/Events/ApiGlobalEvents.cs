using System.Collections.Generic;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS
{
    /// <summary>
    /// Api全局事件类
    /// </summary>
    public abstract class ApiGlobalEvents
    {
        /// <summary>
        /// 服务器初始化完成
        /// </summary>
        public virtual Task OnInit(IEnumerable<ApiDescriptor> apis) => null;
        /// <summary>
        /// 这是第一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnReady(IRequest request) => null;

        /// <summary>
        /// Api未找到
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnApiNotFound(IRequest request) => null;

        /// <summary>
        /// Api创建完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnApiCreated(IRequest request) => null;

        /// <summary>
        /// Api参数解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnArgumentsParsed(IRequest request) => null;

        /// <summary>
        /// Api属性解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnPropertiesParsed(IRequest request) => null;
        
        /// <summary>
        /// 这是最后一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnEnding(IRequest request) => null;

        /// <summary>
        /// 出现未处理的非 <seealso cref="ApiException"/> 异常时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnUnprocessException(IRequest request) => OnException(request);

        /// <summary>
        /// 出现 <see cref="ApiException"/> 时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnApiException(IRequest request) => OnException(request);

        /// <summary>
        /// 默认情况下出现任何异常都触发,但重新<see cref="OnApiException"/> 或 <see cref="OnUnprocessException"/> 后不,对应的异常不会触发该事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnException(IRequest request) => null;

        /// <summary>
        /// 准备返回数据时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public virtual Task OnBeginResponse(IRequest request) => null;
    }

}
