using blqw.MIS.Services;
using System;
using System.Collections.Generic;
using System.Web;
using blqw.MIS.Descriptors;
using System.ComponentModel.Composition;
using System.Linq;

namespace blqw.MIS.MVC.Services
{
    public class Resolver : IResolver
    {
        static Resolver()
        {
            blqw.IOC.MEF.Import(typeof(Resolver));
        }
        [ImportMany]
        private static readonly IBodyParser[] _parsers;

        public void Dispose()
        {

        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "Mvc5-Resolver";

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; } = new NameDictionary();

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public bool RequireClone => false;

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public IService Clone() => new Resolver();

        private string GetMineType(string contentType)
        {
            var index = contentType.IndexOf(';');
            return index < 0 ? contentType : contentType.Remove(index);
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ApiArgument> ParseArguments(IRequest request)
        {
            var req = request.CastRequest();
            var mineType = GetMineType(req.Context.Request.ContentType);
            var parser = _parsers.FirstOrDefault(x => x.Match(mineType));
            if (parser == null)
            {
                throw new ApiRequestException(415, $"服务器无法处理 {mineType} 类型的的实体", null);
            }
            return parser.Parse(req);
        }
        

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ApiProperty> ParseProperties(IRequest request)
        {
            var req = request.CastRequest();
            var param = req.Context.Request;
            foreach (var p in req.Api.Properties)
            {
                var value = (object)param.Headers[p.Name];
                if (value == null)
                {
                    if (p.HasDefaultValue)
                    {
                        yield return new ApiProperty(p.Property, p.DefaultValue);
                    }
                }
                else
                {
                    try
                    {
                        value = value.ChangeType(p.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        throw new ApiRequestPropertyCastException(p.Name, "Header头[{0}] 值有误", ex.Message, ex);
                    }
                    yield return new ApiProperty(p.Property, value);
                }
            }
        }

        /// <summary>
        /// 创建Api所属类的实例
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object CreateApiClassInstance(IRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Api == null) throw new ArgumentException("无法推断ApiClass类型", nameof(request));
            return request.Api.Method.IsStatic ? null : Activator.CreateInstance(request.Api.ApiClass.Type);
        }
        
    }
}
