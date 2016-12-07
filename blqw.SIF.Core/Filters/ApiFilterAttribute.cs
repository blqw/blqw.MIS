using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public abstract class ApiFilterAttribute : Attribute, IApiAttribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiFilterAttribute()
        {

        }

        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="settingString">接口设置</param>
        public ApiFilterAttribute(string settingString)
        {
            SettingString = settingString;
        }

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="settingString">接口设置</param>
        public ApiFilterAttribute(string container, string settingString)
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

        /// <summary>
        /// 该值指示此实例是否等于指定的对象。
        /// </summary>
        /// <param name="attribute"> 要与此实例进行比较 <see cref="ApiFilterAttribute"/>。</param>
        /// <returns></returns>
        public virtual bool Match(ApiFilterAttribute attribute) => GetType() == attribute?.GetType();

        public int OrderNumber { get; set; }
    }
}
