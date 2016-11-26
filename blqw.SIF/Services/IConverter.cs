using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Services
{
    /// <summary>
    /// 转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class  Converter<T> : IConverter<T>
    {
        /// <summary>
        /// 转换器输出的类型
        /// </summary>
        public Type OutputType { get; } = typeof(T);

        /// <summary>
        /// 转换对象到另一种类型,返回转换后的对象,如果转换失败则为null
        /// </summary>
        /// <param name="input">需要转换的对象</param>
        /// <param name="failException">转换中出现的异常</param>
        /// <returns></returns>
        object IConverter.Convert(object input, out Exception failException)
        {
            var result = Convert(input, out failException);
            if (failException != null)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// 转换对象到另一种类型,返回转换后的对象,如果转换失败则为 default(T)
        /// </summary>
        /// <param name="input">需要转换的对象</param>
        /// <param name="failException">转换中出现的异常</param>
        /// <returns></returns>
        public abstract T Convert(object input, out Exception failException);
    }

    /// <summary>
    /// 转换器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConverter<out T>: IConverter
    {
        /// <summary>
        /// 转换对象到另一种类型,返回转换后的对象,如果转换失败则为 default(T)
        /// </summary>
        /// <param name="input">需要转换的对象</param>
        /// <param name="failException">转换中出现的异常</param>
        /// <returns></returns>
        T Convert(object input, out Exception failException);
    }

    /// <summary>
    /// 转换器
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// 转换器输出的类型
        /// </summary>
        Type OutputType { get; }
        /// <summary>
        /// 转换对象到另一种类型,返回转换后的对象,如果转换失败则为null
        /// </summary>
        /// <param name="input">需要转换的对象</param>
        /// <param name="failException">转换中出现的异常</param>
        /// <returns></returns>
        object Convert(object input, out Exception failException);
    }
}
