using blqw.UIF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace blqw.UIF
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
        public static IList<IApiAttribute> FiltrateAttribute(this IEnumerable<IApiAttribute> attributes, IApiContainerProvider provider, bool hasGeneral)
        {
            if (attributes == null)
            {
                return null;
            }
            var container = provider.ContainerId ?? throw new ArgumentNullException("provider.ContainerId");
            if (hasGeneral)
            {
                return attributes.Where(it => it != null).Where(it => it.Container == null || string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return attributes.Where(it => it != null).Where(it => string.Equals(it.Container, container, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// 解析设置
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="settingAttributes"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static IDictionary<string, object> ParseSetting(this IApiContainerProvider provider, IEnumerable<IApiAttribute> settingAttributes)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));
            if (settingAttributes == null) throw new ArgumentNullException(nameof(settingAttributes));
            var parser = GetUsableService(provider.SettingParser, false);
            settingAttributes = settingAttributes.FiltrateAttribute(provider, true);
            if (settingAttributes.Any() == false)
            {
                return null;
            }
            if (parser == null || settingAttributes.Any(it => it.SettingString != null) == false)
            {
                var settings = new NameDictionary();
                settings.MakeReadOnly();
                return settings;
            }

            var baseSetting = ParseGeneralSetting(settingAttributes);

            var result = parser.Parse(settingAttributes.Where(a => a.Container != null).Select(a => a.SettingString));
            if (result.Succeed == false)
            {
                throw new InvalidOperationException($"{parser}异常:{result.Error?.ToString() ?? "错误信息丢失"}");
            }
            foreach (var set in result.Settings)
            {
                baseSetting[set.Key] = set.Value;
            }
            baseSetting.MakeReadOnly();
            return baseSetting;
        }


        /// <summary>
        /// 解析通用api特性中的设置
        /// </summary>
        /// <param name="settingAttributes">特性集合</param>
        /// <returns></returns>
        private static NameDictionary ParseGeneralSetting(this IEnumerable<IApiAttribute> settingAttributes)
        {
            var dict = new NameDictionary();
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

        /// <summary>
        /// 解析设置用的正则表达式
        /// </summary>
        private static readonly Regex _parseRegex = new Regex(@"(?<=^|;)(?<key>(\\=|\\;|[^=;])*)=(?<value>(\\;|\\=|[^=;])*)(?=;|$)");

        /// <summary>
        /// 使用标准方式解析设置字符串
        /// </summary>
        /// <param name="setting">设置字符串,标准格式为 "name1=value1;name2=value2" </param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ParseSetting(this string setting)
        {
            if (string.IsNullOrWhiteSpace(setting))
            {
                yield break;
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



        /// <summary>
        /// 转换当前集合为只读集合
        /// </summary>
        /// <param name="enumerable">枚举器</param>
        /// <returns></returns>
        internal static ReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> enumerable) => new ReadOnlyCollection<T>(enumerable.ToList());

        internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null) return;
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

    }
}
