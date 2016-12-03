using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Services
{
    /// <summary>
    /// 服务扩展方法
    /// </summary>
    public static class ServiceExtensions
    {

        /// <summary>
        /// 解析设置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="settingAttributes"></param>
        /// <returns></returns>
        public static ApiSettings ParseSetting(this IApiContainerServices provider, IEnumerable<IApiAttribute> settingAttributes)
        {
            var parser = GetUsableService(provider.SettingParser, false);
            var container = provider.ContainerId;
            if (settingAttributes.Any(it => it.Container == null || string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return null;
            }
            settingAttributes = settingAttributes.Where(it => string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase)).OrderBy(it => it.Container);
            if (parser == null || settingAttributes.Any(it => it.SettingString != null) == false)
            {
                return new ApiSettings();
            }
            var result = parser.Parse(settingAttributes);
            if (result.Succeed)
            {
                return new ApiSettings(result.Settings);
            }
            throw new InvalidOperationException($"{parser}异常:{result.Error ?? (object)"错误信息丢失"}");
        }

        /// <summary>
        /// 获取可用服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="service">原始服务</param>
        /// <param name="required">服务是否必须</param>
        /// <returns></returns>
        public static T GetUsableService<T>(T service, bool required)
        {
            if (service == null)
            {
                if (required)
                    throw new InvalidOperationException($"没有找到{typeof(T).Name}服务");
                return service;
            }
            var iservice = service as IService;
            if (iservice == null)
            {
                return service;
            }
            if (iservice.RequireClone)
            {
                service = (T)iservice.Clone();
                if (service == null && required)
                {
                    throw new InvalidOperationException($"{iservice}.Clone()返回值为null");
                }
            }
            return service;
        }
    }
}
