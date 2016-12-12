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
    /// 线程同步,安全
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SimplyMap<TKey, TValue>
    {
        public SimplyMap()
        {
            _innerDictionary = new Dictionary<TKey, TValue>();
            _locker = new object();
        }


        public SimplyMap(IEqualityComparer<TKey> comparer)
        {
            _innerDictionary = new Dictionary<TKey, TValue>(comparer);
            _locker = new object();
        }

        private Dictionary<TKey, TValue> _innerDictionary;
        private object _locker;

        public TValue this[TKey key]
        {
            get
            {
                if (_innerDictionary.TryGetValue(key, out var value))
                {
                    return value;
                }
                return default(TValue);
            }
        }

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
                var temp = new Dictionary<TKey, TValue>(_innerDictionary, _innerDictionary.Comparer);
                temp.Add(key, newValue);
                _innerDictionary = temp;
            }
            return newValue;
        }
    }
}
