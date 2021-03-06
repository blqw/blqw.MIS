﻿using blqw.MIS.Session;
using blqw.MIS.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using blqw.MIS.Events;

namespace blqw.MIS
{
    /// <summary>
    /// 当前API调用上下文
    /// </summary>
    public sealed class ApiCallContext
    {
        /// <summary>
        /// 返回值提供程序
        /// </summary>
        private readonly IResultProvider _resultProvider;

        /// <summary>
        /// 初始化API上下文
        /// </summary>
        /// <param name="instance"> api实例类 </param>
        /// <param name="method"> api方法 </param>
        /// <param name="resultProvider"> 返回值提供程序 </param>
        /// <param name="session"> api回话信息 </param>
        /// <param name="logger"> 日志记录器 </param>
        public ApiCallContext(object instance, MethodInfo method, IResultProvider resultProvider, ISession session, ILogger logger)
        {
            _resultProvider = resultProvider ?? throw new ArgumentNullException(nameof(resultProvider));
            ApiInstance = instance ?? throw new ArgumentNullException(nameof(instance));
            Method = method;
            Session = session;
            Parameters = new NameDictionary();
            Properties = new NameDictionary();
            Data = new NameDictionary();
            ID = Guid.NewGuid().ToString("n");
            if (instance is ISupportSession s)
                s.Session = session ?? throw ApiException.NotSupportedSession;
        }

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// 上下文ID
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// API参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; }
        /// <summary>
        /// API属性
        /// </summary>
        public IDictionary<string, object> Properties { get; }
        /// <summary>
        /// API上下文数据
        /// </summary>
        public IDictionary<string, object> Data { get; }
        /// <summary>
        /// API Session
        /// </summary>
        public ISession Session { get; }
        /// <summary>
        /// API实例
        /// </summary>
        public object ApiInstance { get; }
        /// <summary>
        /// API方法
        /// </summary>
        public MethodInfo Method { get; }
        /// <summary>
        /// API返回值
        /// </summary>
        public object Result => _resultProvider.Result;
        /// <summary>
        /// API异常
        /// </summary>
        public Exception Exception => _resultProvider.Exception;
        /// <summary>
        /// API是否有错误
        /// </summary>
        public bool IsError => _resultProvider.IsError;
    }
}
