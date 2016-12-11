using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    internal sealed class AttributeCache<TKey, TMember, TValue>
        where TKey : class
        where TMember : class
        where TValue : Attribute
    {
        private static readonly ICollection<TValue> EmptyValues = new TValue[0];

        public sealed class ValueCache : IEnumerable<TValue>
        {
            public TMember Member { get; set; }
            public readonly Func<object, object> Getter;
            private readonly IList<TValue> _list;
            public ValueCache(object member, IList<TValue> attributes, Func<object, object> getter = null)
            {
                Member = (TMember)member;
                Getter = getter;
                _list = attributes;
                Count = _list?.Count ?? 0;
            }

            public int Count { get; }

            public TValue this[int index] => _list[index];

            public IEnumerator<TValue> GetEnumerator() => _list?.GetEnumerator() ?? EmptyValues.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => _list?.GetEnumerator() ?? EmptyValues.GetEnumerator();
        }

        class GetProvider<I, V> : IServiceProvider
        {
            public GetProvider(PropertyInfo property)
            {
                _get = (Func<I, V>)property.GetMethod.CreateDelegate(typeof(Func<I, V>));
            }
            Func<I, V> _get;
            private object GetValue(object instance)
                => _get((I)instance);

            public object GetService(Type serviceType)
                => (Func<object, object>)GetValue;
        }

        private readonly SimplyMap<TKey, List<ValueCache>> _cache;
        private object _locker;
        public AttributeCache()
        {
            _cache = new SimplyMap<TKey, List<ValueCache>>();
            _locker = new object();
        }

        public List<ValueCache> this[TKey key] => _cache.GetOrAdd(key, CreateList);

        private List<ValueCache> CreateList(TKey key)
        {
            var list = new List<ValueCache>();
            var t = key as Type;
            if (t != null)
            {
                foreach (var p in t.GetTypeInfo().DeclaredProperties)
                {
                    var attr = p.GetCustomAttributes<TValue>().ToList();
                    if (attr.Count == 0) continue;
                    if (p.CanRead)
                    {
                        var service = (IServiceProvider)Activator.CreateInstance(typeof(GetProvider<,>).GetTypeInfo().MakeGenericType(p.DeclaringType, p.PropertyType));
                        var getter = (Func<object, object>)service.GetService(null);
                        list.Add(new ValueCache(p, attr, getter));
                    }
                    else
                    {
                        list.Add(new ValueCache(p, attr, null));
                    }
                }
            }
            else
            {
                foreach (var p in (key as MethodInfo).GetParameters())
                {
                    var attr = p.GetCustomAttributes<TValue>().ToList();
                    if (attr.Count == 0) continue;
                    list.Add(new ValueCache(p, attr));
                }
            }
            return list;
        }
    }
}
