using blqw.SIF.Descriptor;
using blqw.SIF.Services;
using blqw.SIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF
{
    /// <summary>
    /// 用户描述的拓展方法
    /// </summary>
    public static class DescriptorExtensions
    {
        /// <summary>
        /// 调用api,获取返回值
        /// </summary>
        /// <param name="api">接口对象</param>
        /// <param name="dataProvider">Api数据提供程序</param>
        /// <returns></returns>
        public static object Invoke(this ApiDescriptor apiDescriptor, IApiDataProvider dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));

            object instance = null;

            if (apiDescriptor.Method.IsStatic == false)
            {
                instance = Activator.CreateInstance(apiDescriptor.ApiClass.Type);
                foreach (var p in apiDescriptor.Properties)
                {
                    var result = dataProvider.GetProperty(p);
                    if (result.Error != null)
                    {
                        return result.Error;
                    }
                    else if (result.Exists)
                    {
                        p.Setter(instance, result.Value);
                    }
                }
            }

            var args = new Dictionary<string, object>();

            foreach (var p in apiDescriptor.Parameters)
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

            return Validator.IsValid(apiDescriptor.Method, args, false) ?? apiDescriptor.Invoke(instance, args.Values.ToArray());
        }

    }
}
