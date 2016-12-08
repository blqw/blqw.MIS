using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    /// <summary>
    /// 一种安全的以<seealso cref="string"/>类型为键的字典
    /// <para></para>当key不存在时返回null
    /// </summary>
    public class SafeStringDictionary : Dictionary<string, object>, IDictionary<string, object>
    {
        object IDictionary<string, object>.this[string key]
        {
            get => TryGetValue(key, out var value) ? value : null;
            set => base[key] = value;
        }

        public new object this[string key]
        {
            get => TryGetValue(key, out var value) ? value : null;
            set => base[key] = value;
        }
    }
}
