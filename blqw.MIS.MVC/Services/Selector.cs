using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Descriptors;
using blqw.MIS.Services;

namespace blqw.MIS.MVC.Services
{
    public class Selector : ISelector
    {
        private readonly string _urlTemplate;
        private readonly ApiContainer _container;
        public Selector(ApiContainer container, string urlTemplate)
        {
            if (urlTemplate == null) throw new ArgumentNullException(nameof(urlTemplate));
            _urlTemplate = urlTemplate.ToLowerInvariant();
            _container = container ?? throw new ArgumentNullException(nameof(container));
            Data = new NameDictionary();
        }

        public void Dispose()
        {

        }

        public IEnumerable<string> GetAllUrls()
        {
            foreach (var api in _container.Apis)
            {
                yield return _urlTemplate
                    .Replace("{namespace}", api.ApiClass.Type.Namespace)
                    .Replace("{class}", api.ApiClass.Name)
                    .Replace("{method}", api.Name);
            }
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "Owin-Selector";

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
        public IService Clone() => new Selector(_container, _urlTemplate);

        /// <summary>
        /// 返回一个接口描述
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        public ApiDescriptor FindApi(IRequest request)
        {
            var req = (request ?? throw new ArgumentNullException(nameof(request))) as Request;
            if (req == null) throw new ArgumentException($"{nameof(request)}类型只能是{typeof(Request).FullName}", nameof(request));
            var route = req.ActualRequest.RequestContext.RouteData;
            var ns = route.Values["namespace"] as string;
            var cls = route.Values["class"] as string;
            var msd = route.Values["method"] as string;

            var @namespace = ns == null ? null : _container.Namespaces.FirstOrDefault(n => ns.Equals(n.FullName, StringComparison.OrdinalIgnoreCase));
            var type = (@namespace?.Types ?? _container.ApiClasses).FirstOrDefault(t => string.Equals(cls, t.FullName, StringComparison.OrdinalIgnoreCase));
            return type?.Apis.FirstOrDefault(a => string.Equals(msd, a.Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
