using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.Services;
using System.IO;
using blqw.UIF.DataModification;
using blqw.UIF.Filters;
using blqw.UIF.Validation;

namespace blqw.UIF.Owin
{
    class OwinProvider : IApiContainerServices, IConverter, IApiSettingParser
    {

        public IApiSettingParser SettingParser => null;

        static readonly Lazy<IEnumerable<Type>> _typesLazy = new Lazy<IEnumerable<Type>>(GetTypes);
        public IEnumerable<Type> DefinedTypes => _typesLazy.Value;

        public string ContainerId => "Owin";

        public IConverter Converter => this;

        public string Name => nameof(OwinProvider);

        public IDictionary<string, object> Propertise { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public bool RequireClone => false;

        public List<ApiFilterAttribute> GlobalFilters { get; set; } = new List<ApiFilterAttribute>();

        public List<DataValidationAttribute> GlobalValidations { get; set; } = new List<DataValidationAttribute>();

        public List<DataModificationAttribute> GlobalModifications { get; set; } = new List<DataModificationAttribute>();

        IEnumerable<ApiFilterAttribute> IApiContainerServices.GlobalFilters => GlobalFilters;

        IEnumerable<DataValidationAttribute> IApiContainerServices.GlobalValidations => GlobalValidations;

        IEnumerable<DataModificationAttribute> IApiContainerServices.GlobalModifications => GlobalModifications;

        private static IEnumerable<Type> GetTypes()
        {
            var owinDomain = AppDomain.CreateDomain("Owin.UIF");

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
            return new ParseResult(new NameDictionary());
        }
    }
}
