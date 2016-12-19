using blqw.UIF.Descriptor;
using blqw.UIF.Services;
using blqw.UIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.Filters;

namespace blqw.UIF
{
    /// <summary>
    /// 用户描述的拓展方法
    /// </summary>
    public static class DescriptorExtensions
    {
        /// <summary>
        /// 同步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        private static object SyncProcess(this object result)
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
                return t.GetDeclaredProperty("Result").GetValue(task)?.SyncProcess(); //如果是泛型任务,获得Result值
            }
            return null;
        }

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

        /// <summary>
        /// 同步调用api
        /// </summary>
        /// <param name="apiDescriptor"> 接口描述 </param>
        /// <param name="dataProvider"> Api数据提供程序 </param>
        /// <returns></returns>
        public static ApiCallContext Invoke(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var context = apiDescriptor.Container.CreateContext(apiDescriptor, dataProvider, out var resultProvider);
            if (context.IsError) return context;

            var instance = context.ApiInstance;
            var filterArgs = new FilterArgs(context, resultProvider);
            apiDescriptor.FiltrationOnExecuting(context, filterArgs); //执行前置过滤器
            if (filterArgs.Cancel == false)
            {
                filterArgs.Result = apiDescriptor.Invoke(instance, context.Parameters.Values.ToArray()).SyncProcess(); //执行方法
            }
            filterArgs.Cancel = false;
            apiDescriptor.FiltrationOnExecuted(context, filterArgs); //执行后置过滤器
            return context;
        }

        private static void FiltrationOnExecuted(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                filter.OnExecuted(context, filterArgs);
            }
        }

        private static void FiltrationOnExecuting(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                filter.OnExecuting(context, filterArgs);
            }
        }

#pragma warning disable IDE1006 // 命名样式
        /// <summary>
        /// 异步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        private static async Task<object> AsyncProcess(this object result)
#pragma warning restore IDE1006 // 命名样式
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
                return await t.GetDeclaredProperty("Result").GetValue(task)?.AsyncProcess(); //如果是泛型任务,获得Result值
            }
            return null;
        }

        /// <summary>
        /// 异步调用api
        /// </summary>
        /// <param name="api">接口对象</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public static async Task<ApiCallContext> InvokeAsync(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var context = apiDescriptor.Container.CreateContext(apiDescriptor, dataProvider, out var resultProvider);
            if (context.IsError) return context;

            var instance = context.ApiInstance;
            var filterArgs = new FilterArgs(context, resultProvider);
            await apiDescriptor.FiltrationOnExecutingAsync(context, filterArgs); //执行前置过滤器
            if (filterArgs.Cancel == false)
            {
                filterArgs.Result = await apiDescriptor.Invoke(instance, context.Parameters.Values.ToArray()).AsyncProcess(); //执行方法
            }
            filterArgs.Cancel = false;
            await apiDescriptor.FiltrationOnExecutedAsync(context, filterArgs); //执行后置过滤器
            return context;
        }

        private static async Task FiltrationOnExecutedAsync(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                var task = filter.OnExecutedAsync(context, filterArgs);
                if (task != null) await task;
            }
        }

        private static async Task FiltrationOnExecutingAsync(this ApiDescriptor apiDescriptor, ApiCallContext context, FilterArgs filterArgs)
        {
            foreach (var filter in apiDescriptor.Filters)
            {
                var task = filter.OnExecutingAsync(context, filterArgs);
                if (task != null) await task;
            }
        }

        //public static async Task<object> InvokeAsync(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        //{
        //    if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
        //    var error = Build(apiDescriptor, dataProvider, out var instance, out var args); //编译api实例和方法参数
        //    if (error != null) return error.GetException();
        //    error = Validator.IsValid(apiDescriptor.Method, args, false);  //验证参数有效性
        //    if (error != null) return error.GetException();

        //    var filterArgs = new Filters.FilterArgs(apiDescriptor.Method, args);
        //    var result = await instance.FiltrationOnBegin(filterArgs).GetResultAsync(); //执行前置过滤器
        //    if (result == null)
        //    {
        //        result = await apiDescriptor.Invoke(instance, args.Values.ToArray()).GetResultAsync(); //执行方法
        //    }
        //    filterArgs.Result = result;
        //    result = await instance.FiltrationOnEnd(filterArgs).GetResultAsync(); //执行后置过滤器
        //    return result ?? filterArgs.Result;
        //}


        //private static async Task<object> GetResultAsync(this object result)
        //{
        //    var task = result as Task;
        //    if (task == null)
        //    {
        //        var ex = result as Exception;
        //        if (ex != null)
        //        {
        //            return ex.GetException();
        //        }
        //        return result;
        //    }
        //    if (task.Status == TaskStatus.Created)
        //    {
        //        task.Start();
        //    }
        //    try
        //    {
        //        task.Wait();
        //    }
        //    catch
        //    {
        //        return task.Exception.GetException();
        //    }
        //    var t = task.GetType().GetTypeInfo();
        //    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
        //    {
        //        return GetResult(t.GetDeclaredProperty("Result").GetValue(task));
        //    }
        //    return null;
        //}

    }
}
