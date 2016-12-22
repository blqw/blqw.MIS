using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Events;

namespace blqw.MIS.Services
{
    internal sealed class EventCaller
    {
        private readonly ApiGlobal[] _globals;

        private readonly Action<ApiEventArgs>[] _events;

        public EventCaller(ApiGlobal[] globals)
        {
            _globals = globals ?? throw new ArgumentNullException(nameof(globals));
            _events = new Action<ApiEventArgs>[10];
            foreach (var g in globals)
            {
                InitEvent(g);
            }
        }

        private void InitEvent(ApiGlobal global)
        {
            var methods = global.GetType().GetRuntimeMethods().ToArray();
            var ot = typeof(ApiGlobal);
            var args = new[] { typeof(ApiEventArgs) };
            foreach (GlobalEvents e in Enum.GetValues(typeof(GlobalEvents)))
            {
                var m = methods.FirstOrDefault(it => MatchMethod(it, e.ToString()));
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

        private bool MatchMethod(MethodInfo method, string name)
        {
            if (method.Name != name || method.IsVirtual == false) return false;
            var p = method.GetParameters();
            if (p.Length != 1 || p[0].ParameterType != typeof(ApiEventArgs)) return false;
            return method.DeclaringType != typeof(ApiGlobal);
        }

        public void Invoke(GlobalEvents @event, ApiCallContext context)
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

        class UnprocessException : Exception
        {
            public UnprocessException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

    }



    public enum GlobalEvents
    {
        OnBeginRequest = 1,
        OnEndRequest = 2,
        OnBeginResponse = 3,
        OnUnprocessException = 4,
        OnGetSession = 5,
        OnApiException = 6,
        OnBeginInvokeMethod = 7,
        OnApiDataParsed = 8
    }
}
