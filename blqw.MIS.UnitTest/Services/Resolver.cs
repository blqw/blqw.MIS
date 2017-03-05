using blqw.MIS.Services;
using System;
using System.Collections.Generic;

namespace blqw.MIS.UnitTest
{
    public class Resolver : IResolver
    {
        public void Dispose()
        {

        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => "UnitTest-Resolver";

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
            return req.Arguments;
        }

        /// <summary>
        /// 解析属性
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<ApiProperty> ParseProperties(IRequest request)
        {
            var req = CastRequest(request);
            return req.Properties;
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
        
        private static Request CastRequest(IRequest request)
        {
            var req = (request ?? throw new ArgumentNullException(nameof(request))) as Request;
            return req ?? throw new ArgumentException($"{nameof(request)}类型只能是{typeof(Request).FullName}", nameof(request));
        }
    }
}
