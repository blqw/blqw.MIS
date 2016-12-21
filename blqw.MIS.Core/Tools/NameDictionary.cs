using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace blqw.MIS
{
    /// <summary>
    ///     一种的以<seealso cref="string" />类型为键的字典
    ///     <para></para>
    ///     当key不存在时返回null
    /// </summary>
    public class NameDictionary : IDictionary<string, object>
    {
        private IDictionary<string, object> _items;

        public NameDictionary()
        {
            _items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public NameDictionary(StringComparer comparer)
        {
            _items = new Dictionary<string, object>(comparer);
        }

        public object this[string key]
        {
            get { return _items.TryGetValue(key, out var value) ? value : null; }
            set { _items[key] = value; }
        }


        public int Count => _items.Count;

        public bool IsReadOnly => _items.IsReadOnly;

        public ICollection<string> Keys => _items.Keys;

        public ICollection<object> Values => _items.Values;

        public void Add(KeyValuePair<string, object> item) => _items.Add(item);

        public void Add(string key, object value) => _items.Add(key, value);

        public void Clear() => _items.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _items.Contains(item);

        public bool ContainsKey(string key) => _items.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _items.GetEnumerator();

        public bool Remove(KeyValuePair<string, object> item) => _items.Remove(item);

        public bool Remove(string key) => _items.Remove(key);

        public bool TryGetValue(string key, out object value) => _items.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public void MakeReadOnly()
        {
            if (IsReadOnly)
            {
                return;
            }
            _items = new ReadOnlyDictionary<string, object>(_items);
        }
    }
}