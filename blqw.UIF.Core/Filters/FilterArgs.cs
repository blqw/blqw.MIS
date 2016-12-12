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
        private IResultProvider _resultProvider;

        public FilterArgs(ApiCallContext context, IResultProvider resultProvider)
        {
            Context = context;
            _resultProvider = resultProvider;
        }

        /// <summary>
        /// 当前api方法
        /// </summary>
        public ApiCallContext Context { get; }

        /// <summary>
        /// 获取或设置过滤器的返回值
        /// </summary>
        public object Result
        {
            get => Context.Result;
            set
            {
                Cancel = true;
                _resultProvider.Result = value;
            }
        }

        /// <summary>
        /// 获取或设置过滤器的异常
        /// </summary>
        public Exception Exception
        {
            get => Context.Exception;
            set
            {
                Cancel = true;
                _resultProvider.Exception = value;
            }
        }

        /// <summary>
        /// 是否取消当前请求
        /// </summary>
        public bool Cancel { get; set; }
    }
}
