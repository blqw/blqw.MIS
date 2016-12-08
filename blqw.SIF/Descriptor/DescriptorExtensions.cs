using blqw.SIF.Descriptor;
using blqw.SIF.Services;
using blqw.SIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF.Filters;

namespace blqw.SIF
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
            var ex1 = exception as AggregateException;
            if (ex1 != null)
            {
                if (ex1.InnerExceptions.Count == 1)
                {
                    return GetRealException(ex1.InnerException) ?? ex1;
                }
                return ex1;
            }
            var ex2 = exception as TargetInvocationException;
            if (ex2 != null)
            {
                return GetRealException(ex2.InnerException) ?? ex2;
            }
            return exception;
        }


        /// <summary>
        /// 同步调用api,获取返回值
        /// </summary>
        /// <param name="api">接口对象</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public static ApiCallContext Invoke(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var context = apiDescriptor.Container.CreateContext(apiDescriptor, dataProvider, out var resultProvider);
            if (context.IsError) return context;

            foreach (var item in apiDescriptor.Parameters)
            {

            }

            var error = Validator.IsValid(apiDescriptor.Method, context.Parameters, false);  //验证参数有效性
            if (error != null)
            {
                resultProvider.Result = error;
                return context;
            }

            var filterArgs = new FilterArgs(apiDescriptor.Method, args);
            var result = instance.FiltrationOnBegin(filterArgs).SyncProcess(); //执行前置过滤器
            if (filterArgs.Cancel == false)
            {
                result = apiDescriptor.Invoke(instance, args.Values.ToArray()).SyncProcess(); //执行方法
            }
            filterArgs.Result = result;
            filterArgs.Cancel = false;
            result = instance.FiltrationOnEnd(filterArgs).SyncProcess(); //执行后置过滤器
            return result ?? filterArgs.Result;
        }

        private static object FiltrationOnEnd(this object instance, FilterArgs filterArgs)
        {
            return null;
        }

        private static object FiltrationOnBegin(this object instance, FilterArgs filterArgs)
        {
            return null;
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
