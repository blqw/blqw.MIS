using blqw.MIS.Services;
using System;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS
{
    /// <summary>
    /// 调度器
    /// </summary>
    public sealed class Scheduler
    {
        /// <summary>
        /// 服务入口
        /// </summary>
        private readonly IServiceEntry _entry;
        
        /// <summary>
        /// 初始化调度器
        /// </summary>
        /// <param name="entry"></param>
        public Scheduler(IServiceEntry entry)
        {
            _entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }
        /// <summary>
        /// 异步执行调度程序,返回响应实体
        /// </summary>
        /// <param name="requestSetter"></param>
        /// <returns></returns>
        public async Task<IResponse> OnExecuteAsync(IRequestSetter requestSetter)
        {
            if (requestSetter == null) throw new ArgumentNullException(nameof(requestSetter));
            var request = requestSetter.Request;
            if (request == null) throw new ArgumentNullException(nameof(request));
            var api = _entry.Selector.GetServiceInstance(true).FindApi(request);
            if (api == null) return null;
            requestSetter.ApiDescriptor = api;
            var resolver = _entry.Resolver.GetServiceInstance(true);
            requestSetter.Instance = resolver.CreateApiClassInstance(request);
            requestSetter.Properties = resolver.ParseProperties(request)?.AsReadOnly();
            requestSetter.Arguments = resolver.ParseArguments(request)?.AsReadOnly();
            requestSetter.Result = await _entry.Invoker.GetServiceInstance(true).Execute(request).ProcessResultAsync();
            return resolver.GetResponse(request);
        }

        /// <summary>
        /// 出现错误时触发
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ex"></param>
        public void OnError(IRequest request, Exception ex)
        {

        }

        /// <summary>
        /// 响应结束时触发
        /// </summary>
        /// <param name="request"></param>
        public void OnEnd(IRequest request)
        {

        }
    }
}
