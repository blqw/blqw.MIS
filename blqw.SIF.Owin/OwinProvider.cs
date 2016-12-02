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
    class OwinProvider : ApiServiceProvider
    {

        public override IApiSettingParser SettingParser => null;

        static readonly Lazy<IEnumerable<Type>> _typesLazy = new Lazy<IEnumerable<Type>>(GetTypes);
        public override IEnumerable<Type> Types => _typesLazy.Value;

        private static IEnumerable<Type> GetTypes()
        {
            var owinDomain = AppDomain.CreateDomain("Owin.SIF");

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory,"*.dll", SearchOption.AllDirectories)
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

        public override IConverter GetConverter(Type type)
        {
            throw new NotImplementedException();
        }

        public override IConverter<T> GetConverter<T>()
        {
            throw new NotImplementedException();
        }
    }
}
