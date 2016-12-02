using blqw.SIF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF
{
    public abstract class ApiServiceProvider
    {
        public abstract IEnumerable<Type> Types { get; }

        public abstract IApiSettingParser SettingParser { get; }

        public abstract IConverter<T> GetConverter<T>();

        public abstract IConverter GetConverter(Type type);


        public ApiSettings ParseSetting(IEnumerable<IApiAttribute> settingAttributes)
        {
            var parser = GetService(SettingParser, false);
            if (parser == null || settingAttributes.Any(it => it.SettingString != null) == false)
            {
                return new ApiSettings();
            }
            if (parser.TryParse(settingAttributes, out IDictionary<string, object> settings))
            {
                return new ApiSettings(settings);
            }
            throw new InvalidOperationException($"{nameof(IApiSettingParser)}.TryParse:{parser.Name}");
        }

        internal static T GetService<T>(T service, bool required)
        {
            if (service == null)
            {
                if (required)
                    throw new InvalidOperationException($"{nameof(ApiServiceProvider)}没有提供{typeof(T).Name}");
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
                    throw new InvalidOperationException($"{typeof(T).Name}.Clone()返回值为null");
                }
            }
            return service;
        }
    }
}
