using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.MIS
{

    /// <summary>
    /// 表示Api属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public sealed class ApiParameterAttribute : Attribute, IApiAttribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiParameterAttribute()
        {

        }
        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="settingString">接口设置</param>
        public ApiParameterAttribute(string settingString)
        {
            SettingString = settingString;
        }

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="settingString">接口设置</param>
        public ApiParameterAttribute(string container, string settingString)
        {
            Container = container;
            SettingString = settingString;
        }

        /// <summary>
        /// 容器ID
        /// </summary>
        public string Container { get; }

        /// <summary>
        /// 设置字符串
        /// </summary>
        public string SettingString { get; }
    }
}
