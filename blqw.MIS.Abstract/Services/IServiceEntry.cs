namespace blqw.MIS.Services
{
    /// <summary>
    /// 服务入口程序,用于提供各种服务
    /// </summary>
    public interface IServiceEntry
    {
        /// <summary>
        /// 容器
        /// </summary>
        ApiContainer Container { get; }
        /// <summary>
        /// API执行器
        /// </summary>
        IInvoker Invoker { get; }
        /// <summary>
        /// 请求解析器
        /// </summary>
        IResolver Resolver { get; }
        /// <summary>
        /// API选择器
        /// </summary>
        ISelector Selector { get; }
    }
}
