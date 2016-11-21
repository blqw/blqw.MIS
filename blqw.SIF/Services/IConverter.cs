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
    public interface IConverter<out T>: IConverter
    {
        T Convert(object input, out Exception failException);
    }

    /// <summary>
    /// 转换器
    /// </summary>
    public interface IConverter
    {
        Type OutputType { get; }
        object Convert(object input, out Exception failException);
    }
}
