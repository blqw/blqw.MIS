using blqw.SIF.Services;
using blqw.SIF.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    /// 用于描述一个接口
    /// </summary>
    public sealed class ApiDescriptor
    {
        /// <summary>
        /// 初始化接口描述
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        public ApiDescriptor(ApiClassDescriptor apiClass, MethodInfo method)
        {
            Class = apiClass ?? throw new ArgumentNullException(nameof(apiClass));
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Parameters = new ReadOnlyCollection<ApiParameterDescriptor>(method.GetParameters().Select(it => new ApiParameterDescriptor(it)).ToList());
        }

        /// <summary>
        /// 接口方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 接口类
        /// </summary>
        public ApiClassDescriptor Class { get; }

        /// <summary>
        /// 参数描述集合
        /// </summary>
        public ICollection<ApiParameterDescriptor> Parameters { get; }

        /// <summary>
        /// 调用api方法,获取返回值
        /// </summary>
        /// <param name="api">接口对象</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public object Invoke(object api, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));

            if (Method.IsStatic)
            {
                api = null;
            }
            else if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            var args = new Dictionary<string, object>();
            foreach (var p in Parameters)
            {
                var result = dataProvider.GetParameter(p);
                if (result.Exists == false)
                {
                    args.Add(p.Name, p.DefaultValue);
                }
                else if (result.Error != null)
                {
                    return result.Error;
                }
                else
                {
                    args.Add(p.Name, result.Value);
                }
            }

            return Validator.IsValid(Method, args, false) ?? Method.Invoke(api, args.Values.ToArray());
        }
    }
}
