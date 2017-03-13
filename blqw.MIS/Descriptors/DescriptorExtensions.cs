using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace blqw.MIS
{
    /// <summary>
    /// 服务扩展方法
    /// </summary>
    public static class DescriptorExtensions
    {
        /// <summary>
        /// 转换当前集合为只读集合
        /// </summary>
        /// <param name="enumerable">枚举器</param>
        /// <returns></returns>
        public static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) return null;
            return enumerable as IReadOnlyList<T> ?? new ReadOnlyCollection<T>(enumerable as IList<T> ?? enumerable.ToList());
        }

        /// <summary>
        /// 获取真实的异常信息
        /// </summary>
        /// <param name="exception">需要获取真实异常的异常信息</param>
        /// <returns></returns>
        public static Exception ProcessException(this Exception exception)
        {
            if (exception == null) return null;

            switch (exception)
            {
                case AggregateException ex:
                    return (ex.InnerExceptions.Count == 1) ? ProcessException(ex.InnerExceptions[0]) ?? ex : ex;
                case TargetInvocationException ex:
                    return ProcessException(ex.InnerException) ?? ex;
                default:
                    return exception;
            }
        }

        /// <summary>
        /// 同步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        public static object ProcessResult(this object result)
        {
            var task = result as Task;
            if (task == null)
            {
                return (result as Exception)?.ProcessException() ?? result;
            }
            try
            {
                if (task.Status == TaskStatus.Created)
                    task.RunSynchronously(); //如果任务尚未执行,则立即执行
                else
                    task.Wait(); //同步等待返回值
            }
            catch (Exception ex)
            {
                return (task.Exception ?? ex).ProcessException();
            }

            var i = task.GetType();
            if (i == VoidTask)
            {
                return null;
            }
            var t = i.GetTypeInfo();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return ProcessResult(((dynamic)task).Result); //如果是泛型任务,获得Result值
            }
            return null;
        }

        private static readonly Type VoidTask = typeof(Task<>).MakeGenericType(Type.GetType("System.Threading.Tasks.VoidTaskResult"));

        /// <summary>
        /// 异步处理返回值
        /// </summary>
        /// <param name="result">原始返回值</param>
        /// <returns>如果返回值是Task,则返回同步执行后的返回值</returns>
        public static async Task<object> ProcessResultAsync(this object result)
        {
            var task = result as Task;
            if (task == null)
            {
                return (result as Exception)?.ProcessException() ?? result;
            }
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                return (task.Exception ?? ex).ProcessException();
            }

            var t = task.GetType().GetTypeInfo();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return await ProcessResultAsync(((dynamic)task).Result); //如果是泛型任务,获得Result值
            }
            return null;
        }
    }
}
