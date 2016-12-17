using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.Validation;

namespace blqw.SIF.Session
{
    /// <summary>
    /// 表示会话数据
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// 获取当前 Session 是否是全新实例
        /// </summary>
        bool IsNewSession { get; }

        /// <summary>
        /// 获取当前 Session 的实例
        /// </summary>
        string SessionId { get; }

        /// <summary>
        /// 销毁当前 Session 中的数据
        /// </summary>
        void Abandon();

        /// <summary>
        /// 获取或设置当前 Session 中的数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object this[string name] { get; set; }

        /// <summary>
        /// Session 中缓存的个数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 判断 Session 中是否存在指定name的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);
    }
}
