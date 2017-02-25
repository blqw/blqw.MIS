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
        /// 设置字符串
        /// </summary>
        string SettingString { get; }
    }
}