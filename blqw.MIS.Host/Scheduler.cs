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
            var container = _entry.Container;
            try
            {
                await container.OnReadyAsync(request);
                if (request.Result != null) goto LABEL_ENDING;
                var api = _entry.Selector.GetServiceInstance(true).FindApi(request);
                if (api == null)
                {
                    await container.OnApiNotFoundAsync(request);
                    if (request.Result != null) goto LABEL_ENDING;
                    await _entry.Container.OnEndingAsync(request);
                    return request.Result ?? new ApiNotFoundException();
                }
                requestSetter.ApiDescriptor = api;
                var resolver = _entry.Resolver.GetServiceInstance(true);
                requestSetter.Instance = resolver.CreateApiClassInstance(request);
                await container.OnApiCreatedAsync(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Properties = resolver.ParseProperties(request)?.AsReadOnly();
                await container.OnPropertiesParsedAsync(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Arguments = resolver.ParseArguments(request)?.AsReadOnly();
                await container.OnArgumentsParsedAsync(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Result = await _entry.Invoker.GetServiceInstance(true).Execute(request).ProcessResultAsync();
            }
            catch (ApiException ex)
            {
                requestSetter.Result = ex.ProcessException();
                await _entry.Container.OnExceptionAsync(request);
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
                await _entry.Container.OnUnprocessExceptionAsync(request);
            }
            LABEL_ENDING:
            try
            {
                await _entry.Container.OnEndingAsync(request);
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
                await _entry.Container.OnUnprocessExceptionAsync(request);
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
            var container = _entry.Container;
            try
            {
                container.OnReady(request);
                if (request.Result != null) goto LABEL_ENDING;
                var api = _entry.Selector.GetServiceInstance(true).FindApi(request);
                if (api == null)
                {
                    container.OnApiNotFound(request);
                    if (request.Result != null) goto LABEL_ENDING;
                    _entry.Container.OnEnding(request);
                    return request.Result ?? new ApiNotFoundException();
                }
                requestSetter.ApiDescriptor = api;
                var resolver = _entry.Resolver.GetServiceInstance(true);
                requestSetter.Instance = resolver.CreateApiClassInstance(request);
                container.OnApiCreated(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Properties = resolver.ParseProperties(request)?.AsReadOnly();
                container.OnPropertiesParsed(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Arguments = resolver.ParseArguments(request)?.AsReadOnly();
                container.OnArgumentsParsed(request);
                if (request.Result != null) goto LABEL_ENDING;
                requestSetter.Result = _entry.Invoker.GetServiceInstance(true).Execute(request).ProcessResult();
            }
            catch (ApiException ex)
            {
                requestSetter.Result = ex.ProcessException();
                _entry.Container.OnException(request);
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
                _entry.Container.OnUnprocessException(request);
            }
            LABEL_ENDING:
            try
            {
                _entry.Container.OnEnding(request);
            }
            catch (Exception ex)
            {
                requestSetter.Result = ex.ProcessException();
                _entry.Container.OnUnprocessException(request);
            }
            return request.Result;
        }
    }
}
