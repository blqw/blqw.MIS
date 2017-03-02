using blqw.MIS.Descriptor;
using blqw.MIS.Filters;
using blqw.MIS.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 用于描述的拓展方法
    /// </summary>
    public static class DescriptorExtensions
    {
        /// <summary>
        /// 获取真实的异常信息
        /// </summary>
        /// <param name="exception">需要获取真实异常的异常信息</param>
        /// <returns></returns>
        public static Exception GetRealException(this Exception exception)
        {
            if (exception == null) return null;

            switch (exception)
            {
                case AggregateException ex:
                    if (ex.InnerExceptions.Count == 1)
                    {
                        return GetRealException(ex.InnerException) ?? ex;
                    }
                    return ex;
                case TargetInvocationException ex:
                    return GetRealException(ex.InnerException) ?? ex;
                default:
                    return exception;
            }
        }


        #region sync

        /// <summary>
        /// 同步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        private static object ProcessResult(this object result)
        {
            var task = result as Task;
            if (task == null)
            {
                return (result as Exception)?.GetRealException() ?? result;
            }
            try
            {
                if (task.Status == TaskStatus.Created)
                    task.RunSynchronously(); //如果任务尚未执行,则立即执行
                else
                    task.Wait(); //同步等待返回值
            }
            catch
            {
                return task.Exception.GetRealException();
            }

            var t = task.GetType().GetTypeInfo();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return t.GetDeclaredProperty("Result").GetValue(task)?.ProcessResult(); //如果是泛型任务,获得Result值
            }
            return null;
        }

        /// <summary>
        /// 同步调用api
        /// </summary>
        /// <param name="apiDescriptor"> API描述 </param>
        /// <param name="dataProvider"> API数据提供程序 </param>
        /// <returns></returns>
        public static ApiCallContext Invoke(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var container = apiDescriptor.Container;
            var context = container.CreateContext(apiDescriptor, dataProvider, out var resultProvider);
            if (context.IsError) return context;
            try
            {
                var instance = context.ApiInstance;
                var filterArgs = new FilterArgs(context, resultProvider);
                apiDescriptor.FiltrationOnExecuting(context, filterArgs); //执行前置过滤器
                if (filterArgs.Cancel == false)
                {
                    container.EventCaller.Invoke(GlobalEvents.OnBeginInvokeMethod, context);
                    filterArgs.Result = apiDescriptor.Invoke(instance, context.Parameters.Values.ToArray()).ProcessResult(); //执行方法
                }
                else
                {
                    context.Debug("前置过滤器截断");
                }
                filterArgs.Cancel = false;
                apiDescriptor.FiltrationOnExecuted(context, filterArgs); //执行后置过滤器
                container.EventCaller.Invoke(GlobalEvents.OnBeginResponse, context);
                return context;
            }
            catch (Exception ex)
            {
                context.Error(ex, "系统异常");
                container.EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
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
                        container.EventCaller.Invoke(GlobalEvents.OnApiException, context);
                    }
                    else
                    {
                        context.Error(context.Exception, "服务器内部错误");
                        container.EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
                    }
                }
            }
        }

        /// <summary>
        /// 同步执行后置过滤器
        /// </summary>
        /// <param name="apiDescriptor"> API描述 </param>
        /// <param name="context"> 上下文 </param>
        /// <param name="filterArgs"> 过滤器集合 </param>
        private static void FiltrationOnExecuted(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                filter.OnExecuted(context, filterArgs);
            }
        }

        /// <summary>
        /// 同步执行前置过滤器
        /// </summary>
        /// <param name="apiDescriptor"> API描述 </param>
        /// <param name="context"> 上下文 </param>
        /// <param name="filterArgs"> 过滤器集合 </param>
        private static void FiltrationOnExecuting(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                filter.OnExecuting(context, filterArgs);
            }
        }

        #endregion


        #region async

        /// <summary>
        /// 异步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        private static async Task<object> ProcessResultAsync(this object result)
        {
            var task = result as Task;
            if (task == null)
            {
                return (result as Exception)?.GetRealException() ?? result;
            }
            try
            {
                await task;
            }
            catch
            {
                return task.Exception.GetRealException();
            }

            var t = task.GetType().GetTypeInfo();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return await t.GetDeclaredProperty("Result").GetValue(task)?.ProcessResultAsync(); //如果是泛型任务,获得Result值
            }
            return null;
        }

        /// <summary>
        /// 异步调用API
        /// </summary>
        /// <param name="api"> API描述 </param>
        /// <param name="dataProvider"> API数据提供程序 </param>
        /// <returns></returns>
        public static async Task<ApiCallContext> InvokeAsync(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var container = apiDescriptor.Container;
            var context = container.CreateContext(apiDescriptor, dataProvider, out var resultProvider);
            if (context.IsError) return context;

            try
            {
                var instance = context.ApiInstance;
                var filterArgs = new FilterArgs(context, resultProvider);
                await apiDescriptor.FiltrationOnExecutingAsync(context, filterArgs); //执行前置过滤器
                if (filterArgs.Cancel == false)
                {
                    container.EventCaller.Invoke(GlobalEvents.OnBeginInvokeMethod, context);
                    filterArgs.Result = await apiDescriptor.Invoke(instance, context.Parameters.Values.ToArray()).ProcessResultAsync(); //执行方法
                }
                else
                {
                    context.Debug("前置过滤器截断");
                }
                filterArgs.Cancel = false;
                await apiDescriptor.FiltrationOnExecutedAsync(context, filterArgs); //执行后置过滤器
                container.EventCaller.Invoke(GlobalEvents.OnBeginResponse, context);
                return context;
            }
            catch (Exception ex)
            {
                context.Error(ex, "系统异常");
                container.EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
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
                        container.EventCaller.Invoke(GlobalEvents.OnApiException, context);
                    }
                    else
                    {
                        context.Error(context.Exception, "服务器内部错误");
                        container.EventCaller.Invoke(GlobalEvents.OnUnprocessException, context);
                    }
                }
            }
        }

        /// <summary>
        /// 异步执行后置过滤器
        /// </summary>
        /// <param name="apiDescriptor"> API描述 </param>
        /// <param name="context"> 上下文 </param>
        /// <param name="filterArgs"> 过滤器集合 </param>
        private static async Task FiltrationOnExecutedAsync(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                var task = filter.OnExecutedAsync(context, filterArgs);
                if (task != null) await task;
            }
        }

        /// <summary>
        /// 异步执行后置过滤器
        /// </summary>
        /// <param name="apiDescriptor"> API描述 </param>
        /// <param name="context"> 上下文 </param>
        /// <param name="filterArgs"> 过滤器集合 </param>
        private static async Task FiltrationOnExecutingAsync(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                var task = filter.OnExecutingAsync(context, filterArgs);
                if (task != null) await task;
            }
        }

        #endregion
    }
}
