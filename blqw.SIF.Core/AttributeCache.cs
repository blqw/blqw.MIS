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
        static readonly ICollection<TValue> _emptyValues = new TValue[0];

        public sealed class ValueCache : IEnumerable<TValue>
        {
            public TMember Member { get; set; }
            public readonly Func<object, object> Getter;
            private ICollection<TValue> list;
            public ValueCache(object member, IEnumerable<TValue> attributes, Func<object, object> getter = null)
            {
                Member = (TMember)member;
                Getter = getter;
                list = attributes?.ToList();
                Count = list?.Count ?? 0;
            }

            public int Count { get; }

            public IEnumerator<TValue> GetEnumerator() => list?.GetEnumerator() ?? _emptyValues.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => list?.GetEnumerator() ?? _emptyValues.GetEnumerator();
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

        private Dictionary<TKey, List<ValueCache>> _cache;
        private object _locker;
        public AttributeCache()
        {
            _cache = new Dictionary<TKey, List<ValueCache>>();
            _locker = new object();
        }

        public List<ValueCache> this[TKey key]
        {
            get
            {
                if (_cache.TryGetValue(key, out var list))
                {
                    return list;
                }
                lock (_locker)
                {
                    if (_cache.TryGetValue(key, out list))
                    {
                        return list;
                    }
                    var temp = new Dictionary<TKey, List<ValueCache>>(_cache);
                    list = new List<ValueCache>();
                    var t = key as Type;
                    if (t != null)
                    {
                        foreach (var p in t.GetTypeInfo().DeclaredProperties)
                        {
                            var attr = p.GetCustomAttributes<TValue>();
                            if (attr.Any() == false) continue; 
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
                            list.Add(new ValueCache(p, p.GetCustomAttributes<TValue>()));
                        }
                    }
                    _cache = temp;
                }
                return list;
            }
        }
    }
}
