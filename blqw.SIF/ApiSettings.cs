using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    public sealed class ApiSettings : IDictionary<string, object>
    {
        IDictionary<string, object> _dictionary;
        public ApiSettings(IDictionary<string, object> dictionary)
        {
            _dictionary = dictionary;
        }

        public ApiSettings()
        {

        }

        public object this[string key]
            => _dictionary != null && _dictionary.TryGetValue(key, out object value) ? value : null;

        object IDictionary<string, object>.this[string key]
        {
            get => this[key];
            set => throw new NotSupportedException("集合是只读的");
        }

        public int Count => _dictionary?.Count ?? 0;

        public bool IsReadOnly => true;


        static readonly string[] _emptyArray = new string[0];
        public ICollection<string> Keys => _dictionary?.Keys ?? _emptyArray;

        public ICollection<object> Values => _dictionary?.Values ?? _emptyArray;

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            => throw new NotSupportedException("集合是只读的");

        void IDictionary<string, object>.Add(string key, object value)
            => throw new NotSupportedException("集合是只读的");

        public void Clear()
            => throw new NotSupportedException("集合是只读的");

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            => _dictionary?.Contains(item) ?? false;

        public bool ContainsKey(string key)
            => _dictionary.ContainsKey(key);
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            => _dictionary?.CopyTo(array, arrayIndex);

        static readonly IEnumerator<KeyValuePair<string, object>> _emptyEnumerator = new List<KeyValuePair<string, object>>().GetEnumerator();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            => _dictionary?.GetEnumerator() ?? _emptyEnumerator;

        public bool Remove(KeyValuePair<string, object> item)
            => throw new NotSupportedException("集合是只读的");

        public bool Remove(string key)
            => throw new NotSupportedException("集合是只读的");

        public bool TryGetValue(string key, out object value)
        {
            value = null;
            return  _dictionary?.TryGetValue(key, out value) ?? false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
