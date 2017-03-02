using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;

namespace blqw.MIS.Services
{
    /// <summary>
    /// 默认API调用器
    /// </summary>
    public class DefaultInvoker : IInvoker
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static DefaultInvoker Instance { get; } = new DefaultInvoker();

        private DefaultInvoker()
        {
            Data = new NameDictionary();
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "默认API调用器";

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; }

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public bool RequireClone => false;

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public IService Clone() => new DefaultInvoker();

        /// <summary>
        /// 执行调用器得到返回值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        object IInvoker.Execute(IRequest request)
            => Execute(request as IRequestBase ?? throw new ArgumentException("参数必须实现IRequestBase", nameof(request)));
        
        /// <summary>
        /// 执行调用器得到返回值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Execute(IRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            SetProperty(request);
            var args = GetArguments(request);
            return request.ApiDescriptor.Invoke(request.Instance, args);
        }

        private static object[] GetArguments(IRequestBase request)
        {
            var args = new object[request.Arguments.Count];
            foreach (var arg in request.Arguments)
            {
                args[arg.Parameter.Position] = arg.Value;
            }
            return args;
        }

        private static void SetProperty(IRequestBase request)
        {
            var props1 = request.ApiDescriptor.Properties;
            var props2 = request.Properties;
            for (var i = 0; i < props1.Count; i++)
            {
                var p = props1[i];
                var rp = props2[i];
                if (p.Property != rp.Property)
                {
                    rp = props2.FirstOrDefault(x => x.Property == p.Property);
                }
                if (rp.Property != null)
                {
                    p.SetValue(request.Instance, rp.Value);
                }
                else if (p.HasDefaultValue)
                {
                    p.SetValue(request.Instance, p.DefaultValue);
                }
            }
        }
    }
}
