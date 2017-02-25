namespace blqw.MIS
{
    /// <summary>
    /// 定义API接口基类
    /// </summary>
    public interface IApiAttribute
    {
        /// <summary>
        /// 容器ID
        /// </summary>
        string Container { get;}

        /// <summary>
        /// 初始化数据
        /// </summary>
        string InitializeData { get; }
    }
}