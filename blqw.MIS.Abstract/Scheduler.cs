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
        public async Task<object> ExecuteAsync(IRequestSetter requestSetter)
        {
            if (requestSetter == null) throw new ArgumentNullException(nameof(requestSetter));
            var request = requestSetter.Request;
            if (request == null) throw new ArgumentNullException(nameof(request));
            var api = _entry.Selector.GetServiceInstance(true).FindApi(request);
            if (api == null) return new ApiNotFoundException();
            requestSetter.ApiDescriptor = api;
            var resolver = _entry.Resolver.GetServiceInstance(true);
            try
            {
                requestSetter.Instance = resolver.CreateApiClassInstance(request);
                requestSetter.Properties = resolver.ParseProperties(request)?.AsReadOnly();
                requestSetter.Arguments = resolver.ParseArguments(request)?.AsReadOnly();
                requestSetter.Result = await _entry.Invoker.GetServiceInstance(true).Execute(request).ProcessResultAsync();
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
            }
            return request.Result;
        }

        /// <summary>
        /// 同步执行调度程序,返回响应实体
        /// </summary>
        /// <param name="requestSetter"></param>
        /// <returns></returns>
        public object Execute(IRequestSetter requestSetter)
        {
            if (requestSetter == null) throw new ArgumentNullException(nameof(requestSetter));
            var request = requestSetter.Request;
            if (request == null) throw new ArgumentNullException(nameof(request));
            var api = _entry.Selector.GetServiceInstance(true).FindApi(request);
            if (api == null) return new ApiNotFoundException();
            requestSetter.ApiDescriptor = api;
            var resolver = _entry.Resolver.GetServiceInstance(true);
            try
            {
                requestSetter.Instance = resolver.CreateApiClassInstance(request);
                requestSetter.Properties = resolver.ParseProperties(request)?.AsReadOnly();
                requestSetter.Arguments = resolver.ParseArguments(request)?.AsReadOnly();
                requestSetter.Result = _entry.Invoker.GetServiceInstance(true).Execute(request).ProcessResult();
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
            }
            return request.Result;
        }
    }
}
