using blqw.SIF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace blqw.SIF
{
    /// <summary>
    /// 服务扩展方法
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 过滤特性
        /// </summary>
        /// <param name="attributes">特性枚举</param>
        /// <param name="provider">容器</param>
        /// <param name="hasGeneral">是否包含通用特性</param>
        /// <returns></returns>
        public static IEnumerable<IApiAttribute> FiltrationAttribute(this IEnumerable<IApiAttribute> attributes, IApiContainerServices provider, bool hasGeneral)
        {
            var container = provider.ContainerId;
            if (hasGeneral)
            {
                return attributes.Where(it => it.Container == null || string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase));
            }
            return attributes.Where(it => string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 解析设置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="settingAttributes"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ParseSetting(this IApiContainerServices provider, IEnumerable<IApiAttribute> settingAttributes)
        {
            var parser = GetUsableService(provider.SettingParser, false);
            settingAttributes = settingAttributes.FiltrationAttribute(provider, true);
            if (settingAttributes.Any() == false)
            {
                return null;
            }
            if (parser == null || settingAttributes.Any(it => it.SettingString != null) == false)
            {
                return new Dictionary<string, object>();
            }

            var baseSetting = ParseGeneralSetting(settingAttributes);
            
            var result = parser.Parse(settingAttributes.Where(a => a.Container != null));
            if (result.Succeed)
            {
                foreach (var set in result.Settings)
                {
                    baseSetting[set.Key] = set.Value;
                }
                return baseSetting;
            }
            throw new InvalidOperationException($"{parser}异常:{result.Error ?? (object)"错误信息丢失"}");
        }



        public static IDictionary<string, object> ParseGeneralSetting(this IEnumerable<IApiAttribute> settingAttributes)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var attr in settingAttributes)
            {
                if (attr.Container != null || string.IsNullOrWhiteSpace(attr.SettingString))
                {
                    continue;
                }
                foreach (var set in ParseSetting(attr.SettingString))
                {
                    dict[set.Key] = set.Value;
                }
            }
            return dict;
        }

        private static readonly Regex _parseRegex = new Regex(@"(?<=^|;)(?<key>(\\=|\\;|[^=;])*)=(?<value>(\\;|\\=|[^=;])*)(?=;|$)");
        

        public static IEnumerable<KeyValuePair<string, string>> ParseSetting(this string setting)
        {
            if (string.IsNullOrWhiteSpace(setting))
            {
                yield break ;
            }
            foreach (Match m in _parseRegex.Matches(setting))
            {
                var key = Regex.Unescape(m.Groups["key"].Value);
                var value = Regex.Unescape(m.Groups["value"].Value);
                yield return new KeyValuePair<string, string>(key, value);
            }
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


        /// <summary>
        /// 根据键安全的获取字典中的值
        /// </summary>
        /// <param name="dictionary">需要获取值的字典</param>
        /// <param name="key">指定的键</param>
        /// <param name="defaultValue">当字典为空或键不存在时返回的值</param>
        /// <returns>字典为空或键不存在时返回 <paramref name="defaultValue"/>，否则获取键对应的值</returns>
        public static object SafeGet(this IDictionary<string, object> dictionary, string key, object defaultValue = null)
            => dictionary != null && dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    }
}
