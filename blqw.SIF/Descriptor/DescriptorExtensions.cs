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
        /// 编译得到api的实例和方法参数
        /// </summary>
        /// <param name="apiDescriptor">api描述</param>
        /// <param name="dataProvider">数据提供程序</param>
        /// <param name="instance">api实例</param>
        /// <param name="args">api参数</param>
        /// <returns>如果编译失败,返回异常信息</returns>
        private static Exception Build(ApiDescriptor apiDescriptor, IApiDataProvider dataProvider, out object instance, out Dictionary<string, object> args)
        {
            if (apiDescriptor.Method.IsStatic == false)
            {
                instance = Activator.CreateInstance(apiDescriptor.ApiClass.Type);
                foreach (var p in apiDescriptor.Properties)
                {
                    var result = dataProvider.GetProperty(p);
                    if (result.Error != null)
                    {
                        instance = null;
                        args = null;
                        return result.Error;
                    }
                    else if (result.Exists)
                    {
                        p.Setter(instance, result.Value);
                    }
                }
            }
            else
            {
                instance = null;
            }

            args = new Dictionary<string, object>();

            foreach (var p in apiDescriptor.Parameters)
            {
                var result = dataProvider.GetParameter(p);
                if (result.Exists == false)
                {
                    args.Add(p.Name, p.DefaultValue);
                }
                else if (result.Error != null)
                {
                    instance = null;
                    args = null;
                    return result.Error;
                }
                else
                {
                    args.Add(p.Name, result.Value);
                }
            }

            return null;
        }

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
            catch (Exception)
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
        private static Exception GetRealException(this Exception exception)
        {
            var ex1 = exception as AggregateException;
            if (ex1 != null)
            {
                if (ex1.InnerExceptions.Count == 1)
                {
                    return GetRealException(ex1.InnerException);
                }
                return ex1;
            }
            var ex2 = exception as TargetInvocationException;
            if (ex2 != null)
            {
                return GetRealException(ex2.InnerException);
            }
            return exception;
        }


        /// <summary>
        /// 同步调用api,获取返回值
        /// </summary>
        /// <param name="api">接口对象</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public static object Invoke(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            var error = Build(apiDescriptor, dataProvider, out var instance, out var args); //api实例和方法参数
            if (error != null) return error.GetRealException();
            error = Validator.IsValid(apiDescriptor.Method, args, false);  //验证参数有效性
            if (error != null) return error.GetRealException();

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
