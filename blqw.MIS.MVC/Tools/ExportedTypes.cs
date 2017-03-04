using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace blqw.MIS.MVC
{
    /// <summary>
    /// 获取展开类型
    /// </summary>
    internal class ExportedTypes
    {
        public static IEnumerable<Type> Enumerable()
        {
            var tempDomain = AppDomain.CreateDomain("MIS");

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe", SearchOption.AllDirectories)))
            {
                Assembly ass;
                try
                {
                    var bytes = File.ReadAllBytes(file);
                    ass = tempDomain.Load(bytes);
                    if (IsValid(ass) == false)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
        /// <summary>
        /// 过滤系统组件,动态组件,全局缓存组件
        /// </summary>
        /// <param name="assembly"> </param>
        /// <returns> </returns>
        protected static bool IsValid(Assembly assembly)
        {
            return (assembly.IsDynamic == false)
                   && (assembly.ManifestModule.Name != "<未知>")
                   && (assembly.GlobalAssemblyCache == false)
                   && (assembly.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) == false)
                   && (assembly.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) == false);
        }

        protected static bool IsValid(Type type)
        {
            return (type?.FullName != null)
                   && (type.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) == false)
                   && (type.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) == false)
                   && (_validChars.IsMatch(type.Name));
        }

        private static readonly Regex _validChars = new Regex("^[a-z][0-9a-z_]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
