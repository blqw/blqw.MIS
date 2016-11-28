using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF
{
    /// <summary>
    /// 表示可以用于对外提供接口的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ApiAttribute : Attribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiAttribute()
        {
            
        }
        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="settingString">接口设置</param>
        public ApiAttribute(string settingString)
        {
            SettingString = settingString;
        }

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="settingString">接口设置</param>
        public ApiAttribute(string container, string settingString)
        {
            Container = container;
            SettingString = settingString;
        }
        public string Container { get; set; }
        public string SettingString { get; set; }
    }
}
