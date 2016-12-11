using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.DataModification
{
    public static class Modifier
    {

        private static readonly AttributeCache<MethodInfo, ParameterInfo, DataModificationAttribute> MethodCache = new AttributeCache<MethodInfo, ParameterInfo, DataModificationAttribute>();
        private static readonly AttributeCache<Type, PropertyInfo, DataModificationAttribute> TypeCache = new AttributeCache<Type, PropertyInfo, DataModificationAttribute>();


        /// <summary>
        /// 根据规则修改方法中参数的值
        /// </summary>
        /// <param name="method">指定的方法</param>
        /// <param name="args">方法参数</param>
        public static void Modifies(MethodInfo method, IDictionary<string, object> args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (args == null) throw new ArgumentNullException(nameof(args));
            var paras = MethodCache[method];
            for (var i = 0; i < paras.Count; i++)
            {
                var p = paras[i];
                if (args.TryGetValue(p.Member.Name, out var value))
                {
                    for (var j = 0; j < p.Count; j++)
                    {
                        p[j].Modifies(ref value);
                    }
                    Modifies(value, null);
                }
            }
        }

        public static void Modifies(object instance)
            => Modifies(instance, null);

        public static void Modifies(object instance, ApiCallContext context)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            var props = TypeCache[instance.GetType()];
            for (var i = 0; i < props.Count; i++)
            {
                var p = props[i];
                var value = p.Getter(instance);
                for (var j = 0; j < p.Count; j++)
                {
                    p[j].Modifies(ref value, context);
                }
                Modifies(value, context);
            }
        }
    }
}
