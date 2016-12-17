using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Filters
{
    /// <summary>
    /// 过滤器参数
    /// </summary>
    public sealed class FilterArgs
    {
        /// <summary>
        /// 返回值更新程序
        /// </summary>
        private readonly IResultUpdater _resultUpdater;

        public FilterArgs(ApiCallContext context, IResultUpdater resultUpdater)
        {
            Context = context;
            _resultUpdater = resultUpdater;
        }

        /// <summary>
        /// 当前api方法
        /// </summary>
        public ApiCallContext Context { get; }

        /// <summary>
        /// 获取或设置返回值，设置该属性会将 <see cref="Cancel"/> 属性设置为true
        /// </summary>
        public object Result
        {
            get => Context.Result;
            set
            {
                Cancel = true;
                _resultUpdater.SetResult(value);
            }
        }

        /// <summary>
        /// 获取或设置异常，设置该属性会将 <see cref="Cancel"/> 属性设置为true
        /// </summary>
        public Exception Exception
        {
            get => Context.Exception;
            set
            {
                Cancel = true;
                _resultUpdater.SetException(value);
            }
        }

        /// <summary>
        /// 是否取消当前请求
        /// </summary>
        public bool Cancel { get; set; }
    }
}
