using blqw.MIS.DataModification;
using blqw.MIS.Descriptor;
using blqw.MIS.Filters;
using blqw.MIS.Logging;
using blqw.MIS.Services;
using blqw.MIS.Session;
using blqw.MIS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace blqw.MIS
{
    /// <summary>
    /// API容器
    /// </summary>
    public sealed class ApiContainer
    {
        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="id">容器名称,唯一标识</param>
        /// <param name="provider">容器组件提供程序</param>
        public ApiContainer(string id, IApiContainerProvider provider)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
            var filters = new List<ApiFilterAttribute>();
            var validations = new List<DataValidationAttribute>();
            var modifications = new List<DataModificationAttribute>();
            if (provider.GlobalFilters != null) filters.AddRange(provider.GlobalFilters);
            if (provider.GlobalValidations != null) validations.AddRange(provider.GlobalValidations);
            if (provider.GlobalModifications != null) modifications.AddRange(provider.GlobalModifications);

            var apiGlobalType = typeof(ApiGlobal).GetTypeInfo();
            var apiGlobals = provider.DefinedTypes
                                .Select(t => t.GetTypeInfo())
                                .Where(t => t.IsAbstract == false)
                                .Where(t => apiGlobalType.IsAssignableFrom(t))
                                .Where(t => t.IsGenericType == false || t.IsGenericTypeDefinition == false)
                                .Select(t => t.DeclaredConstructors.FirstOrDefault(c => c.GetParameters().Length == 0))
                                .Where(c => c != null)
                                .Select(c => (ApiGlobal)c.Invoke(null))
                                .ToArray();

            foreach (var apiGlobal in apiGlobals)
            {
                apiGlobal.Initialization();
                if (apiGlobal.Filters != null) filters.AddRange(apiGlobal.Filters);
                if (apiGlobal.Validations != null) validations.AddRange(apiGlobal.Validations);
                if (apiGlobal.Modifications != null) modifications.AddRange(apiGlobal.Modifications);
            }
            Filters = filters.AsReadOnly();
            Validations = validations.AsReadOnly();
            Modifications = modifications.AsReadOnly();
            ApiCollection = new ApiCollection(this);
            EventCaller = new EventCaller(apiGlobals);
        }

        /// <summary>
        /// 事件处理器
        /// </summary>
        public EventCaller EventCaller { get; }

        /// <summary>
        /// 容器id
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// 接口集合
        /// </summary>
        public ApiCollection ApiCollection { get; }

        /// <summary>
        /// 服务集合
        /// </summary>
        public IApiContainerProvider Provider { get; }

        /// <summary>
        /// 全局过滤器
        /// </summary>
        public IReadOnlyCollection<ApiFilterAttribute> Filters { get; }

        /// <summary>
        /// 全局验证器
        /// </summary>
        public IReadOnlyCollection<DataValidationAttribute> Validations { get; }

        /// <summary>
        /// 全局数据变更器
        /// </summary>
        public IReadOnlyCollection<DataModificationAttribute> Modifications { get; }


        /// <summary>
        /// 创建上下文
        /// </summary>
        /// <param name="apiDescriptor"> api描述 </param>
        /// <param name="dataProvider"> 数据提供程序 </param>
        /// <param name="resultUpdater"> 返回值更新程序 </param>
        /// <returns>如果编译失败,返回异常信息</returns>
        public ApiCallContext CreateContext(ApiDescriptor apiDescriptor, IApiDataProvider dataProvider, out IResultUpdater resultUpdater)
        {
            if (ApiCollection.Apis.Contains(apiDescriptor ?? throw new ArgumentNullException(nameof(apiDescriptor))) == false)
            {
                throw new ArgumentException("API描述不属于当前容器", nameof(apiDescriptor));
            }
            var logger = Provider.Logger.GetUsableService(false); //获取日志服务
            ApiCallContext context = null;
            try
            {
                var instance = apiDescriptor.Method.IsStatic
                    ? null
                    : dataProvider.GetApiInstance(apiDescriptor) ??
                      Activator.CreateInstance(apiDescriptor.ApiClass.Type);
                resultUpdater = Provider.CreateResultUpdater() ?? new ResultProvider();
                var session = dataProvider.GetSession();
                context = new ApiCallContext(instance, apiDescriptor.Method, resultUpdater, session, logger);
                context.Data["$ResultProvider"] = resultUpdater;
                context.Data["$ApiContainer"] = this;
                context.Data["$ApiDescriptor"] = apiDescriptor;

                logger?.Append(new LogItem()
                {
                    Context = context,
                    Level = LogLevel.Debug,
                    Message = "创建上下文",
                    Title = "系统",
                });

                if (session != null) EventCaller.Invoke(GlobalEvents.OnBeginRequest, context);

                var parameters = context.Parameters;
                var properties = context.Properties;

                //属性
                if (apiDescriptor.Method.IsStatic == false)
                {
                    foreach (var p in apiDescriptor.Properties)
                    {
                        var result = dataProvider.GetProperty(p);

                        if (result.Error != null && result.Exists)
                        {
                            properties.Add(p.Name, result.Error);
                            resultUpdater.SetException(result.Error);
                            return context;
                        }

                        if (result.Exists || p.HasDefaultValue == false)
                        {
                            var value = result.Exists ? result.Value : null;
                            if (p.DataModifications.Count > 0)
                                p.DataModifications.ForEach(it => it.Modifies(ref value, context)); //变更数据
                            Modifier.Modifies(value, context);
                            if (result.Exists)
                            {
                                properties.Add(p.Name, value);
                                p.Setter(instance, value);
                            }
                            if (p.DataValidations.Count > 0)
                            {
                                var ex = p.DataValidations.FirstOrDefault(it => it.IsValid(value, context) == false)?
                                             .GetException(p.Name, value, context)
                                         ?? Validator.IsValid(value, context, true); //数据验证
                                if (ex != null)
                                {
                                    resultUpdater.SetException(ex);
                                    return context;
                                }
                            }
                        }
                        else
                        {
                            properties.Add(p.Name, p.DefaultValue);
                            p.Setter(instance, p.DefaultValue);
                        }

                    }
                }

                //参数
                foreach (var p in apiDescriptor.Parameters)
                {
                    if (p.ParameterType == typeof(ISession))
                    {
                        parameters.Add(p.Name, session);
                        continue;
                    }
                    if (p.ParameterType == typeof(ApiCallContext))
                    {
                        parameters.Add(p.Name, context);
                        continue;
                    }
                    if (p.ParameterType == typeof(ILogger))
                    {
                        parameters.Add(p.Name, logger);
                        continue;
                    }

                    var result = dataProvider.GetParameter(p);

                    if (result.Error != null && result.Exists)
                    {
                        parameters.Add(p.Name, result.Error);
                        resultUpdater.SetException(result.Error);
                        return context;
                    }

                    if (result.Exists)
                    {
                        var value = result.Value;
                        if (p.DataModifications.Count > 0)
                            p.DataModifications.ForEach(it => it.Modifies(ref value, context)); //变更数据
                        Modifier.Modifies(value, context);
                        parameters.Add(p.Name, value);
                        if (p.DataValidations.Count > 0)
                        {
                            var ex = p.DataValidations.FirstOrDefault(it => it.IsValid(value, context) == false)?.GetException(p.Name, value, context)
                                     ?? Validator.IsValid(value, context, true); //数据验证
                            if (ex != null)
                            {
                                resultUpdater.SetException(ex);
                                return context;
                            }
                        }
                    }
                    else if (p.HasDefaultValue)
                    {
                        parameters.Add(p.Name, p.DefaultValue);
                    }
                    else
                    {
                        var ex = ApiException.ArgumentMissing(p.Name);
                        parameters.Add(p.Name, ex);
                        resultUpdater.SetException(ex);
                        return context;
                    }
                }

                EventCaller.Invoke(GlobalEvents.OnApiDataParsed, context);
                return context;
            }
            catch (Exception ex)
            {
                logger?.Append(new LogItem()
                {
                    Context = context,
                    Exception = ex,
                    Level = LogLevel.Critical,
                    Message = "未处理异常",
                    Title = "系统",
                });
                EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
                throw;
            }
            finally
            {
                if (context?.IsError == true)
                {
                    var ex = context.Exception.GetRealException();
                    if (ex is ApiException e)
                    {
                        context.Warning(e.Message, "逻辑异常");
                        if (e.InnerException != null)
                        {
                            context.Warning(e.InnerException, "内部逻辑异常");
                        }
                        EventCaller.Invoke(GlobalEvents.OnApiException, context);
                    }
                    else
                    {
                        context.Error(context.Exception, "服务器内部错误");
                        EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
                    }
                }
            }
        }

        /// <summary>
        /// 将API上下文中的 <see cref="Parameters"/> 和 <see cref="Properties"/> 变为只读集合
        /// </summary>
        /// <param name="context"> api上下文 </param>
        public void MakeReadOnlyContext(ApiCallContext context)
        {
            ((NameDictionary)context?.Parameters).MakeReadOnly();
            ((NameDictionary)context?.Properties).MakeReadOnly();
        }

        /// <summary>
        /// 触发请求开始事件
        /// </summary>
        public void OnBeginRequest() => EventCaller.Invoke(GlobalEvents.OnBeginRequest, null);

        /// <summary>
        /// 触发请求结束事件
        /// </summary>
        /// <param name="context"></param>
        public void OnEndRequest(ApiCallContext context) => EventCaller.Invoke(GlobalEvents.OnBeginRequest, context);
    }
}

