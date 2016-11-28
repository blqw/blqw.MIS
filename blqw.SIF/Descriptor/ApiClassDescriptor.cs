using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using blqw.SIF.Services;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    /// 表示一个Api类
    /// </summary>
    public class ApiClassDescriptor
    {
        public Type Type { get; }
        /// <summary>
        /// 当前Api类的类名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 当前Api类的完整类名
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// 返回当前Api类的实例,如果当前类是静态类或抽象类,则返回null,如果<seealso cref="IApiDataProvider"/>提供的参数无法正确调用构造函数,则抛出异常
        /// </summary>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public object CreateInstance(IApiDataProvider dataProvider)
            => Activator.CreateInstance(Type);
    }
}
