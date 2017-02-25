using System;

namespace blqw.MIS
{

    /// <summary>
    /// 表示API属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class ApiPropertyAttribute : Attribute, IApiAttribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiPropertyAttribute() { }
        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="initializeData">接口设置</param>
        public ApiPropertyAttribute(string initializeData)
            => InitializeData = initializeData;

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="initializeData">初始化数据</param>
        public ApiPropertyAttribute(string container, string initializeData)
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
