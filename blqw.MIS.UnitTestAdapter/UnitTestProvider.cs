using blqw.MIS.NetFramework45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    class UnitTestProvider : ApiContainerProvider
    {
        public UnitTestProvider() 
            : base("UnitTest",GetTypes().ToList())
        {
        }

        private static IEnumerable<Type> GetTypes()
        {
            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsValid(ass) == false)
                {
                    continue;
                }
                foreach (var m in ass.Modules)
                {
                    IEnumerable<Type> types;
                    try
                    {
                        types = m.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    foreach (var t in types)
                    {
                        if (IsValid(t))
                            yield return t;
                    }
                }
            }
        }
        
    }
}
