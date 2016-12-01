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
        public abstract Assembly[] Assemblies { get; }

        public abstract IApiSettingParser SettingParser { get; }

        public abstract IConverter<T> GetConverter<T>();

        public abstract IConverter GetConverter(Type type);


        public ApiSettings ParseSetting(IEnumerable<IApiAttribute> settingAttributes)
        {
            if (settingAttributes.Any() == false)
            {
                return new ApiSettings();
            }
            var parser = GetService(SettingParser);
            if (parser.TryParse(settingAttributes, out IDictionary<string, object> settings))
            {
                return new ApiSettings(settings);
            }
            throw new InvalidOperationException($"{nameof(IApiSettingParser)}.TryParse:{parser.Name}");
        }

        internal static T GetService<T>(T service)
        {
            if (service == null)
                throw new InvalidOperationException($"{nameof(ApiServiceProvider)}没有提供{typeof(T).Name}");
            var iservice = service as IService;
            if (iservice == null)
            {
                return service;
            }
            if (iservice.RequireClone)
            {
                service = (T)iservice.Clone();
                if (service == null)
                {
                    throw new InvalidOperationException($"{typeof(T).Name}.Clone()返回值为null");
                }
            }
            return service;
        }
    }
}
