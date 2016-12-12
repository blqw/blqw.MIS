using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Validation
{
    /// <summary> 
    /// 集合长度验证
    /// </summary>
    public class CountAttribute : DataValidationAttribute
    {
        /// <summary> 
        /// 设置集合的有效长度来初始化特性(无法验证<seealso cref="string"/>)
        /// </summary>
        /// <param name="maxCount">集合的允许的最大长度(包含)</param>
        /// <param name="minCount">集合的允许的最小长度(包含)</param>
        public CountAttribute(int maxCount, int minCount = 0)
            : base(-108, minCount <= 0 ? $"集合参数 {{name}} 数据项不能大于{maxCount}" : $"集合参数 {{name}} 数据项不能大于{maxCount}或小于{minCount}")
        {
            MaxCount = maxCount;
            MinCount = minCount;
        }

        /// <summary> 
        /// 集合的允许的最大长度
        /// </summary>
        public int MaxCount { get; }

        /// <summary> 
        /// 集合的允许的最小长度
        /// </summary>
        public int MinCount { get; }


        public override bool IsValid(object value, IDictionary<string, object> args)
        {
            var list = value as ICollection;
            if (list == null)
            {
                return true;
            }
            if (list.Count > MaxCount || list.Count < MinCount)
            {
                return false;
            }
            if (value is IEnumerable == false || value is string)
            {
                return true;
            }

            var t = value.GetType().GetTypeInfo();
            var getCount = _getCountFunctions[t];
            if (getCount != null)
            {
                var count = getCount(value);
                return count >= MinCount && count <= MaxCount;
            }
            foreach (var item in t.ImplementedInterfaces)
            {
                var itype = item.GetTypeInfo();
                if (itype.IsGenericType && itype.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    getCount = _getCountFunctions.GetOrAdd(itype, CreateGetCountFunction);
                    _getCountFunctions.GetOrAdd(t, getCount);
                    var count = getCount(value);
                    return count >= MinCount && count <= MaxCount;
                }
            }
            return true;
        }

        static Func<object, int> CreateGetCountFunction(TypeInfo type)
        {
            var service = typeof(GetCountService<>).MakeGenericType(type.GenericTypeParameters[0]);
            return (Func<object, int>)((IServiceProvider)Activator.CreateInstance(service)).GetService(null);
        }

        public override string GetDescription(Type type)
        {
            if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return MinCount <= 0 ? $"集合不能大于{MaxCount}" : $"集合不能大于{MaxCount}或小于{MinCount}";
            }
            return null;
        }

        static readonly SimplyMap<TypeInfo, Func<object, int>> _getCountFunctions = new SimplyMap<TypeInfo, Func<object, int>>();

        class GetCountService<T> : IServiceProvider
        {
            public int GetCount(object collection) => (collection as ICollection<T>)?.Count ?? -1;
            public object GetService(Type serviceType) => (Func<object, int>)GetCount;
        }
    }
}
