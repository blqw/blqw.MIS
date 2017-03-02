using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Owin.Exceptions;
using blqw.MIS.Services;

namespace blqw.MIS.Owin.Services
{
    public class Resolver : IResolver
    {
        public void Dispose()
        {

        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "Owin-Resolver";

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

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ApiArgument> ParseArguments(IRequest request)
        {
            var req = CastRequest(request);
            var param = new RequestParams(req.OwinContext);
            foreach (var p in req.ApiDescriptor.Parameters)
            {
                var value = (object)param.Body.Get(p.Name) ?? param.Query.Get(p.Name);
                if (value == null)
                {
                    if (p.IsEntity)
                    {
                        //TODO:balabala
                        throw new NotSupportedException("暂不支持实体参数");
                    }
                    else if (p.HasDefaultValue)
                    {
                        yield return new ApiArgument(p.Parameter, p.DefaultValue);
                    }
                    else
                    {
                        throw new RequestArgumentNotFoundException(p.Name);
                    }
                }
                else
                {
                    try
                    {
                        value = value.ChangeType(p.ParameterType);
                    }
                    catch (Exception ex)
                    {
                        var e = ex;
                        while (e.InnerException != null) e = e.InnerException;
                        throw new RequestArgumentCastException(p.Name, detail: e.Message, innerException: ex);
                    }
                    yield return new ApiArgument(p.Parameter, value);
                }
            }

        }

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ApiProperty> ParseProperties(IRequest request)
        {
            var req = CastRequest(request);
            var param = new RequestParams(req.OwinContext);
            foreach (var p in req.ApiDescriptor.Properties)
            {
                var value = (object)param.Header.Get(p.Name);
                if (value == null)
                {
                    if (p.HasDefaultValue == false)
                        throw new RequestArgumentNotFoundException(p.Name);
                    yield return new ApiProperty(p.Property, p.DefaultValue);
                }
                else
                {
                    try
                    {
                        value = value.ChangeType(p.PropertyType);
                    }
                    catch (Exception ex)
                    {
                        throw new RequestArgumentCastException(p.Name, detail: ex.Message, innerException: ex);
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
            if (request.Method.DeclaringType == null) throw new ArgumentException("无法推断ApiClass类型", nameof(request));
            return request.Method.IsStatic ? null : Activator.CreateInstance(request.Method.DeclaringType);
        }

        /// <summary>
        /// 获取响应体
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IResponse GetResponse(IRequest request)
        {
            var req = CastRequest(request);
            if (req.Result is Exception ex)
            {
                throw ex;
            }
            return new Response(req);
        }


        private static Request CastRequest(IRequest request)
        {
            var req = (request ?? throw new ArgumentNullException(nameof(request))) as Request;
            return req ?? throw new ArgumentException($"{nameof(request)}类型只能是{typeof(Request).FullName}", nameof(request));
        }
    }
}
