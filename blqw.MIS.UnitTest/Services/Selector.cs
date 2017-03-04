using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;
using blqw.MIS.Services;

namespace blqw.MIS.UnitTest
{
    public class Selector : ISelector
    {
        private readonly ApiContainer _container;

        public Selector(ApiContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Data = new NameDictionary();
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "UnitTest-Selector";

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
        public IService Clone() => new Selector(_container);

        /// <summary>
        /// 返回一个接口描述
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        public ApiDescriptor FindApi(IRequest request)
        {
            var req = (request ?? throw new ArgumentNullException(nameof(request))) as Request;
            if (req == null) throw new ArgumentException($"{nameof(request)}类型只能是{typeof(Request).FullName}", nameof(request));
            return _container.Apis.FirstOrDefault(x => x.Method == req.Method);
        }
    }
}
