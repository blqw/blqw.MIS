using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF.Session;

namespace blqw.MIS.NetFramework45
{
    /// <summary>
    /// 基于内存缓存 <seealso cref="MemoryCache"/> 的Session实现
    /// </summary>
    public class MemorySession : ISession
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly ObjectCache _cache = new MemoryCache("blqw.MIS.NetFramework45.Session");

        private NameDictionary _dictionary;
        private Func<string> _getNewSessionId;
        public MemorySession(string sessionid, int expireSeconds, Func<string> getNewSessionId)
        {
            _getNewSessionId = getNewSessionId ?? throw new ArgumentNullException(nameof(getNewSessionId));
            SessionId = string.IsNullOrWhiteSpace(sessionid) ? getNewSessionId() : sessionid;
            _dictionary = _cache[SessionId] as NameDictionary;
            if (_dictionary == null)
            {
                _dictionary = new NameDictionary();
                _cache.Add(SessionId, _dictionary, new CacheItemPolicy
                {
                    SlidingExpiration = TimeSpan.FromSeconds(expireSeconds),
                });
                IsNewSession = true;
            }
        }

        /// <summary>
        /// 获取当前 Session 是否是全新实例
        /// </summary>
        public bool IsNewSession { get; private set; }

        /// <summary>
        /// 获取当前 Session 的实例的 Id
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// 销毁当前 Session 中的数据
        /// </summary>
        public void Abandon()
        {
            _cache.Remove(SessionId);
            _dictionary.Clear();
            SessionId = _getNewSessionId();
            IsNewSession = true;
        }
        /// <summary>
        /// 获取或设置当前 Session 中的数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get => _dictionary[name];
            set => _dictionary[name] = value;
        }

        /// <summary>
        /// Session 中缓存的个数
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// 判断 Session 中是否存在指定name的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
            => _dictionary.ContainsKey(name);
    }
}
