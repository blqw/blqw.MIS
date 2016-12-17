using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF
{
    /// <summary>
    /// 线程安全的只读存储
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadOnlyStore<TKey, TValue>
    {
        /// <summary>
        /// 初始化存储实例
        /// </summary>
        public ReadOnlyStore()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
            _locker = new object();
        }

        /// <summary>
        /// 初始化存储实例，并提供键的比较方法
        /// </summary>
        /// <param name="comparer"></param>
        public ReadOnlyStore(IEqualityComparer<TKey> comparer)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(comparer);
            _locker = new object();
        }

        private Dictionary<TKey, TValue> _innerDictionary;
        private readonly object _locker;

        /// <summary>
        /// 获取键对应的值，如果对应值不存在，则返回 default(TValue)
        /// </summary>
        /// <param name="key">用于获取值的键</param>
        /// <returns></returns>
        public TValue this[TKey key] => _innerDictionary.TryGetValue(key, out var value) ? value : default(TValue);

        /// <summary>
        /// 获取或添加值到存储
        /// </summary>
        /// <param name="key">用于获取值的键</param>
        /// <param name="getValue">当值不存在时用于生成值的方法</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> getValue)
        {
            if (_innerDictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            lock (_locker)
            {
                if (_innerDictionary.TryGetValue(key, out value))
                {
                    return value;
                }
                var temp = new Dictionary<TKey, TValue>(_innerDictionary, _innerDictionary.Comparer);
                value = getValue(key);
                temp.Add(key, value);
                _innerDictionary = temp;
            }
            return value;
        }

        /// <summary>
        /// 获取或添加值到存储
        /// </summary>
        /// <param name="key">用于获取值的键</param>
        /// <param name="newValue">当值不存在时，将该值写入存储</param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, TValue newValue)
        {
            if (_innerDictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            lock (_locker)
            {
                if (_innerDictionary.TryGetValue(key, out value))
                {
                    return value;
                }
                _innerDictionary = new Dictionary<TKey, TValue>(_innerDictionary, _innerDictionary.Comparer) { [key] = newValue };
            }
            return newValue;
        }
    }
}
