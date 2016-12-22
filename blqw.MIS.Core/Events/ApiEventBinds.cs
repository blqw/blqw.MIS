using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Events;

namespace blqw.MIS
{
    public abstract partial class ApiGlobal
    {
        /// <summary>
        /// 请求开始时触发,这是第一个被触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeginRequest(ApiEventArgs e) { }

        /// <summary>
        /// 获取 Session 时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnGetSession(ApiEventArgs e) { if (e.Context.Session.IsNewSession) OnNewSession(e); }

        /// <summary>
        /// 获取 Session 时,如果 IsNewSession = true 则触发, 重写 <see cref="OnGetSession"/> 后不触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewSession(ApiEventArgs e) { }

        /// <summary>
        /// 数据解析完成时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnApiDataParsed(ApiEventArgs e) { }

        /// <summary>
        /// 准备调用Api方法时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeginInvokeMethod(ApiEventArgs e) { }

        /// <summary>
        /// 出现未处理的非 <seealso cref="ApiException"/> 异常时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUnprocessException(ApiEventArgs e) => OnException(e);

        /// <summary>
        /// 出现 <see cref="ApiException"/> 时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnApiException(ApiEventArgs e) => OnException(e);

        /// <summary>
        /// 默认情况下出现任何异常都触发,但重新<see cref="OnApiException"/> 或 <see cref="OnUnprocessException"/> 后不,对应的异常不会触发该事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnException(ApiEventArgs e) { }

        /// <summary>
        /// 准备返回数据时触发
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBeginResponse(ApiEventArgs e) { }

        /// <summary>
        /// 请求结束时触发,这是最后一个被触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnEndRequest(ApiEventArgs e) { }

    }

}
