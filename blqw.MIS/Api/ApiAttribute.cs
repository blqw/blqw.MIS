using System;

namespace blqw.MIS
{
    /// <summary>
    /// 表示可以用于对外提供接口的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ApiAttribute : Attribute, IApiAttribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiAttribute() { }

        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="initializeData">初始化数据</param>
        public ApiAttribute(string initializeData)
            => InitializeData = initializeData;

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="initializeData">初始化数据</param>
        public ApiAttribute(string container, string initializeData)
            : this(initializeData)
            => Container = container;

        /// <summary>
        /// 容器ID
        /// </summary>
        public string Container { get; }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public string InitializeData { get; }
    }
}
