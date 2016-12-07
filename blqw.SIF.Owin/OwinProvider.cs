using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF.Services;
using System.IO;

namespace blqw.SIF.Owin
{
    class OwinProvider : IApiContainerServices, IConverter, IApiSettingParser
    {

        public IApiSettingParser SettingParser => null;

        static readonly Lazy<IEnumerable<Type>> _typesLazy = new Lazy<IEnumerable<Type>>(GetTypes);
        public IEnumerable<Type> ApiTypes => _typesLazy.Value;

        public string ContainerId => "Owin";

        public IConverter Converter => this;

        public string Name => nameof(OwinProvider);

        public IDictionary<string, object> Propertise { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public bool RequireClone => false;

        private static IEnumerable<Type> GetTypes()
        {
            var owinDomain = AppDomain.CreateDomain("Owin.SIF");

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories)
                                .Union(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe", SearchOption.AllDirectories)))
            {
                Assembly ass;
                try
                {
                    var bytes = File.ReadAllBytes(file);
                    ass = owinDomain.Load(bytes);
                    if (ass.IsDynamic)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }

                foreach (var t in ass.ExportedTypes)
                {
                    yield return t;
                }
            }
        }

        public ConvertResult ChangeType(object value, Type conversionType)
            => Convert3.TryChangedType(value, conversionType, out var r) ?
                    new ConvertResult(r) : new ConvertResult(new InvalidCastException());

        public IService Clone() => this;

        public void Dispose()
        {

        }

        public ParseResult Parse(IEnumerable<IApiAttribute> settingAttributes)
        {
            return new ParseResult(new Dictionary<string, object>());
        }
    }
}
