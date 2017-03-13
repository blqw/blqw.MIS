using blqw.MIS.Descriptors;
using blqw.MIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// API容器
    /// </summary>
    public sealed class ApiContainer
    {
        private readonly List<NamespaceDescriptor> _namespaces;
        private readonly List<ApiClassDescriptor> _types;
        private readonly List<ApiDescriptor> _apis;
        private readonly List<ApiGlobalEvents> _events;
        private static readonly Dictionary<string, Func<ApiGlobalEvents, IRequest, Task>> EventHandles = InitEventHandles();

        private static Dictionary<string, Func<ApiGlobalEvents, IRequest, Task>> InitEventHandles()
        {
            var events = new Dictionary<string, Func<ApiGlobalEvents, IRequest, Task>>();

            foreach (var method in typeof(ApiGlobalEvents).GetRuntimeMethods())
            {
                if (method.IsStatic == false && method.IsPublic && method.ReturnType == typeof(Task))
                {
                    var args = method.GetParameters();
                    if (args.Length == 1 && args[0].ParameterType == typeof(IRequest))
                    {
                        events.Add(method.Name, (Func<ApiGlobalEvents, IRequest, Task>)method.CreateDelegate(typeof(Func<ApiGlobalEvents, IRequest, Task>)));
                    }
                }
            }
            return events;
        }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="name">容器名称</param>
        /// <param name="exportedTypes">输出类型</param>
        /// <param name="factory">API类描述创建服务</param>
        public ApiContainer(string name, IEnumerable<Type> exportedTypes, IApiClassDescriptorFactory factory = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            _namespaces = new List<NamespaceDescriptor>();
            _types = new List<ApiClassDescriptor>();
            _apis = new List<ApiDescriptor>();
            var @base = typeof(ApiGlobalEvents).GetTypeInfo();
            _events = exportedTypes
                            .Select(t => t.GetTypeInfo())
                            .Where(t => @base.IsAssignableFrom(t) && t.IsAbstract == false && t.ContainsGenericParameters == false)
                            .Select(t => (ApiGlobalEvents)Activator.CreateInstance(t.AsType()))
                            .ToList();

            FindAllApis(exportedTypes, factory);


            Namespaces = _namespaces.AsReadOnly();
            ApiClasses = _types.AsReadOnly();
            Apis = _apis.AsReadOnly();
        }

        /// <summary>
        /// 容器名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 命名空间集合
        /// </summary>
        public IReadOnlyList<NamespaceDescriptor> Namespaces { get; }
        /// <summary>
        /// API类集合
        /// </summary>
        public IReadOnlyList<ApiClassDescriptor> ApiClasses { get; }
        /// <summary>
        /// API集合
        /// </summary>
        public IReadOnlyList<ApiDescriptor> Apis { get; }


        /// <summary>
        /// 查找所有输出类型中的API
        /// </summary>
        /// <param name="exportedTypes">输出类型</param>
        /// <param name="factory">API类描述创建服务</param>
        private void FindAllApis(IEnumerable<Type> exportedTypes, IApiClassDescriptorFactory factory)
        {
            if (exportedTypes == null) throw new ArgumentNullException(nameof(exportedTypes));
            if (factory == null) factory = DefaultIApiClassDescriptorFactory.Instance;
            foreach (var t in exportedTypes)
            {
                var apiclass = factory.Create(t, this);
                if (apiclass == null)
                {
                    continue;
                }
                _types.Add(apiclass);
                var ns = _namespaces.FirstOrDefault(it => it.FullName == t.Namespace);
                if (ns == null)
                {
                    _namespaces.Add(ns = new NamespaceDescriptor(t.Namespace, this));

                }
                ns.AddApiCalss(apiclass);
                _apis.AddRange(apiclass.Apis);
            }
        }

        private async Task OnEventAsync(IRequest request, string eventName)
        {
            if (EventHandles.TryGetValue(eventName, out var ent))
            {
                foreach (var e in _events)
                {
                    try
                    {
                        var task = ent(e, request);
                        if (task?.IsCompleted == false)
                        {
                            await task;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new NotImplementedException($"{e.GetType().FullName}.{eventName}方法异常,详见异常内部说明", ex);
                    }
                }
            }
        }


        /// <summary>
        /// 这是第一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnReadyAsync(IRequest request) => OnEventAsync(request, nameof(OnReady));

        /// <summary>
        /// Api未找到
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnApiNotFoundAsync(IRequest request) => OnEventAsync(request, nameof(OnApiNotFound));

        /// <summary>
        /// Api创建完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnApiCreatedAsync(IRequest request) => OnEventAsync(request, nameof(OnApiCreated));

        /// <summary>
        /// Api参数解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnArgumentsParsedAsync(IRequest request) => OnEventAsync(request, nameof(OnArgumentsParsed));

        /// <summary>
        /// Api属性解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnPropertiesParsedAsync(IRequest request) => OnEventAsync(request, nameof(OnPropertiesParsed));

        /// <summary>
        /// 这是最后一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnEndingAsync(IRequest request) => OnEventAsync(request, nameof(OnEnding));

        /// <summary>
        /// 出现未处理的非 <seealso cref="ApiException"/> 异常时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnUnprocessExceptionAsync(IRequest request) => OnEventAsync(request, nameof(OnUnprocessException));

        /// <summary>
        /// 出现 <see cref="ApiException"/> 时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnApiExceptionAsync(IRequest request) => OnEventAsync(request, nameof(OnApiException));

        /// <summary>
        /// 默认情况下出现任何异常都触发,但重新<see cref="OnApiException"/> 或 <see cref="OnUnprocessException"/> 后不,对应的异常不会触发该事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnExceptionAsync(IRequest request) => OnEventAsync(request, nameof(OnException));

        /// <summary>
        /// 准备返回数据时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public Task OnBeginResponseAsync(IRequest request) => OnEventAsync(request, nameof(OnBeginResponse));


        private void OnEvent(IRequest request, string eventName)
        {
            if (EventHandles.TryGetValue(eventName, out var ent))
            {
                foreach (var e in _events)
                {
                    try
                    {
                        var task = ent(e, request);
                        if (task?.IsCompleted == false)
                        {
                            task.Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new NotImplementedException($"{e.GetType().FullName}.{eventName}方法异常,详见异常内部说明", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 这是第一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnReady(IRequest request) => OnEvent(request, nameof(OnReady));

        /// <summary>
        /// Api未找到
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnApiNotFound(IRequest request) => OnEvent(request, nameof(OnApiNotFound));

        /// <summary>
        /// Api创建完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnApiCreated(IRequest request) => OnEvent(request, nameof(OnApiCreated));

        /// <summary>
        /// Api参数解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnArgumentsParsed(IRequest request) => OnEvent(request, nameof(OnArgumentsParsed));

        /// <summary>
        /// Api属性解析完成
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnPropertiesParsed(IRequest request) => OnEvent(request, nameof(OnPropertiesParsed));

        /// <summary>
        /// 这是最后一个被触发的事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnEnding(IRequest request) => OnEvent(request, nameof(OnEnding));

        /// <summary>
        /// 出现未处理的非 <seealso cref="ApiException"/> 异常时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnUnprocessException(IRequest request) => OnEvent(request, nameof(OnUnprocessException));

        /// <summary>
        /// 出现 <see cref="ApiException"/> 时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnApiException(IRequest request) => OnEvent(request, nameof(OnApiException));

        /// <summary>
        /// 默认情况下出现任何异常都触发,但重新<see cref="OnApiException"/> 或 <see cref="OnUnprocessException"/> 后不,对应的异常不会触发该事件
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnException(IRequest request) => OnEvent(request, nameof(OnException));

        /// <summary>
        /// 准备返回数据时触发
        /// </summary>
        /// <param name="request">当前请求</param>
        public void OnBeginResponse(IRequest request) => OnEvent(request, nameof(OnBeginResponse));
    }
}

