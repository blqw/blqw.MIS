using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public abstract class ApiFilterAttribute : Attribute, IApiAttribute
    {
        /// <summary>
        /// 初始化接口特性
        /// </summary>
        public ApiFilterAttribute()
        {
            var type = this.GetType().GetTypeInfo();
            _overridedExecuting = type.GetDeclaredMethod("OnExecuting").DeclaringType != typeof(ApiFilterAttribute);
            _overridedExecuted = type.GetDeclaredMethod("OnExecuted").DeclaringType != typeof(ApiFilterAttribute);
            _overridedExecutingAsync = type.GetDeclaredMethod("OnExecutingAsync").DeclaringType != typeof(ApiFilterAttribute);
            _overridedExecutedAsync = type.GetDeclaredMethod("OnExecutedAsync").DeclaringType != typeof(ApiFilterAttribute);
        }

        /// <summary>
        /// 使用指定设置字符串初始化接口特性
        /// </summary>
        /// <param name="settingString">接口设置</param>
        public ApiFilterAttribute(string settingString)
            :this()
        {
            SettingString = settingString;
        }

        /// <summary>
        /// 初始化针对特定容器的接口特性
        /// </summary>
        /// <param name="container">指定容器</param>
        /// <param name="settingString">接口设置</param>
        public ApiFilterAttribute(string container, string settingString)
            : this(settingString)
        {
            Container = container;
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

        /// <summary>
        /// 过滤器排序
        /// </summary>
        public int OrderNumber { get; set; }
        /// <summary>
        /// 是否已重写 OnExecuting 
        /// </summary>
        private readonly bool _overridedExecuting;
        /// <summary>
        /// 是否已重写 OnExecuted 
        /// </summary>
        private readonly bool _overridedExecuted;
        /// <summary>
        /// 是否已重写 OnExecutingAsync 
        /// </summary>
        private readonly bool _overridedExecutingAsync;
        /// <summary>
        /// 是否已重写 OnExecutedAsync 
        /// </summary>
        private readonly bool _overridedExecutedAsync;
        
        public virtual void OnExecuting(ApiCallContext context, FilterArgs args)
        {
            if (_overridedExecuting == false && _overridedExecutingAsync)
            {
                OnExecutingAsync(context, args).Wait();
            }
        }
        
        public virtual void OnExecuted(ApiCallContext context, FilterArgs args)
        {
            if (_overridedExecuted  == false && _overridedExecutedAsync)
            {
                OnExecutedAsync(context, args).Wait();
            }
        }

        public virtual Task OnExecutingAsync(ApiCallContext context, FilterArgs args)
        {
            if (_overridedExecuting && _overridedExecutingAsync == false)
            {
                OnExecuting(context, args);
            }
            return null;
        }

        public virtual Task OnExecutedAsync(ApiCallContext context, FilterArgs args)
        {
            if (_overridedExecuted && _overridedExecutedAsync == false)
            {
                OnExecuted(context, args);
            }
            return null;
        }

    }
}
