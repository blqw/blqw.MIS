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
        /// <summary>
        /// 内部存储
        /// </summary>
        private IDictionary<string, object> _items;

        /// <summary>
        /// 初始化字典
        /// </summary>
        public NameDictionary()
            => _items = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 初始化字典,并设置键的比较器
        /// </summary>
        /// <param name="comparer"></param>
        public NameDictionary(StringComparer comparer)
            => _items = new Dictionary<string, object>(comparer);

        /// <summary>
        /// 获取或设置字典中的元素
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get => _items.TryGetValue(key, out var value) ? value : null;
            set => _items[key] = value;
        }

        /// <summary>
        /// 获取字典元素个数
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// 获取字典是否为只读
        /// </summary>
        public bool IsReadOnly => _items.IsReadOnly;

        /// <summary>
        /// 获取字典中键的集合
        /// </summary>
        public ICollection<string> Keys => _items.Keys;

        /// <summary>
        /// 获取字典中值的集合
        /// </summary>
        public ICollection<object> Values => _items.Values;

        /// <summary>
        /// 将元素添加到字典
        /// </summary>
        /// <param name="item">需要添加到字典的元素</param>
        public void Add(KeyValuePair<string, object> item) => _items.Add(item);

        /// <summary>
        /// 将键和值添加到字典
        /// </summary>
        /// <param name="key">需要添加到字典的键</param>
        /// <param name="value">需要添加到字典的值</param>
        public void Add(string key, object value) => _items.Add(key, value);

        /// <summary>
        /// 将字典的元素全部清除
        /// </summary>
        public void Clear() => _items.Clear();

        /// <summary>
        /// 确定字典中是否包含指定的元素
        /// </summary>
        /// <param name="item">需要确定的是否存在的元素</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, object> item) => _items.Contains(item);

        /// <summary>
        /// 确定字典中是否包含指定的键
        /// </summary>
        /// <param name="key">需要确定是否存在的键</param>
        /// <returns></returns>
        public bool ContainsKey(string key) => _items.ContainsKey(key);


        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// 将特定元素从字典中移除
        /// </summary>
        /// <param name="item">待移除的元素</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, object> item) => _items.Remove(item);

        /// <summary>
        /// 从字典中移除特定键对应的元素
        /// </summary>
        /// <param name="key">待移除元素的键</param>
        /// <returns></returns>
        public bool Remove(string key) => _items.Remove(key);

        /// <summary>
        /// 尝试获取指定键的值,返回操作是否成功
        /// </summary>
        /// <param name="key">待获取值的键</param>
        /// <param name="value">操作成功返回的值</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out object value) => _items.TryGetValue(key, out value);


        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        /// <summary>
        /// 将当前字典转为只读
        /// </summary>
        public void MakeReadOnly()
        {
            if (IsReadOnly) return;
            lock (this)
            {
                if (IsReadOnly) return;
                _items = new ReadOnlyDictionary<string, object>(_items);
            }
        }
    }
}