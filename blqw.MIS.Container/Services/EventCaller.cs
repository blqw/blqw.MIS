using blqw.MIS.Events;
using System;
using System.Linq;
using System.Reflection;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 事件调用器
    /// </summary>
    public sealed class EventCaller
    {
        /// <summary>
        /// <seealso cref="ApiGlobal"/> 集合
        /// </summary>
        private readonly ApiGlobal[] _globals;

        /// <summary>
        /// 从 <see cref="_globals"/> 中提取的所有事件
        /// </summary>
        private readonly Action<ApiEventArgs>[] _events;

        /// <summary>
        /// 初始化事件调用器
        /// </summary>
        /// <param name="globals"></param>
        public EventCaller(ApiGlobal[] globals)
        {
            _globals = globals ?? throw new ArgumentNullException(nameof(globals));
            _events = new Action<ApiEventArgs>[10];
            foreach (var g in globals)
            {
                InitEvent(g);
            }
        }

        /// <summary>
        /// 为单独的 <seealso cref="ApiGlobal"/> 初始化事件列表
        /// </summary>
        /// <param name="global"></param>
        private void InitEvent(ApiGlobal global)
        {
            var methods = global.GetType().GetRuntimeMethods().ToArray();
            var ot = typeof(ApiGlobal);
            var args = new[] { typeof(ApiEventArgs) };
            foreach (GlobalEvents e in Enum.GetValues(typeof(GlobalEvents)))
            {
                var m = methods.FirstOrDefault(it => it.Name == e.ToString() && IsOverrided(it));
                if (m == null) continue;
                var ev = _events[(int)e];
                if (ev == null)
                {
                    _events[(int)e] = (Action<ApiEventArgs>)m.CreateDelegate(typeof(Action<ApiEventArgs>), global);
                }
                else
                {
                    _events[(int)e] = ev + (Action<ApiEventArgs>)m.CreateDelegate(typeof(Action<ApiEventArgs>), global);
                }
            }
        }

        /// <summary>
        /// 判断一个方法是否已经被重写了
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private bool IsOverrided(MethodInfo method)
        {
            if (method.IsVirtual == false) return false;
            var p = method.GetParameters();
            if (p.Length != 1 || p[0].ParameterType != typeof(ApiEventArgs)) return false;
            return method.DeclaringType != typeof(ApiGlobal);
        }

        /// <summary>
        /// 触发指定事件
        /// </summary>
        /// <param name="event"></param>
        /// <param name="context"></param>
        internal void Invoke(GlobalEvents @event, ApiCallContext context)
        {
            var i = (int)@event;
            if (i < 0 || i >= _events.Length) return;

            if (@event == GlobalEvents.OnUnprocessException && context != null && context.IsError && context.Exception is UnprocessException)
            {
                return; //防止递归触发事件
            }
            try
            {
                _events[i]?.Invoke(new ApiEventArgs(context));
            }
            catch (Exception ex)
            {
                var msg = @event + "事件异常";
                context.Error(ex, msg);
                var newEx = @event == GlobalEvents.OnUnprocessException ?
                            (Exception)new UnprocessException(msg, ex) :
                            new TargetInvocationException(msg, ex);
                throw newEx;
            }
        }

        /// <summary>
        /// 防止重复触发异常处理事件
        /// </summary>
        private class UnprocessException : Exception
        {
            public UnprocessException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

    }

    /// <summary>
    /// 事件枚举
    /// </summary>
    public enum GlobalEvents
    {
        /// <summary>
        /// 请求开始
        /// </summary>
        OnBeginRequest = 1,
        /// <summary>
        /// 请求结束
        /// </summary>
        OnEndRequest = 2,
        /// <summary>
        /// 准备返回请求响应
        /// </summary>
        OnBeginResponse = 3,
        /// <summary>
        /// 当遭遇未处理的异常
        /// </summary>
        OnUnprocessException = 4,
        /// <summary>
        /// 获取Session
        /// </summary>
        OnGetSession = 5,
        /// <summary>
        /// 当出现 <seealso cref="ApiException"/>
        /// </summary>
        OnApiException = 6,
        /// <summary>
        /// 准备调用API服务方法
        /// </summary>
        OnBeginInvokeMethod = 7,
        /// <summary>
        /// 请求参数解析完成
        /// </summary>
        OnApiDataParsed = 8
    }
}
