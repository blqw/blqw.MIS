using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.DataModification;
using blqw.UIF.Filters;
using blqw.UIF.Services;
using blqw.UIF.Validation;
using System.Text.RegularExpressions;

namespace blqw.UIF.NetFramework45
{
    /// <summary>
    /// 基于 .NET Framework 4.5 实现的容器提供程序
    /// </summary>
    public class ApiContainerProvider : IApiContainerProvider
    {
        protected ApiContainerProvider(string containerId)
            : this(containerId, GetTypes().ToList())
        {

        }

        protected ApiContainerProvider(string containerId, IEnumerable<Type> definedTypes)
        {
            ContainerId = containerId;
            DefinedTypes = definedTypes;
        }

        /// <summary>
        /// 容器ID
        /// </summary>
        public string ContainerId { get; }

        /// <summary>
        /// 获取定义类型的集合
        /// </summary>
        public IEnumerable<Type> DefinedTypes { get; }

        /// <summary>
        /// 设置解析器
        /// </summary>
        public virtual IApiSettingParser SettingParser { get; } = new SettingParser();

        /// <summary>
        /// 类型转换器
        /// </summary>
        public virtual IConverter Converter { get; } = new Convert3Proxy();

        /// <summary>
        /// 全局过滤器
        /// </summary>
        public List<ApiFilterAttribute> GlobalFilters { get; set; } = new List<ApiFilterAttribute>();

        /// <summary>
        /// 全局验证器
        /// </summary>
        public List<DataValidationAttribute> GlobalValidations { get; set; } = new List<DataValidationAttribute>();

        /// <summary>
        /// 全局数据修改器
        /// </summary>
        public List<DataModificationAttribute> GlobalModifications { get; set; } = new List<DataModificationAttribute>();

        IEnumerable<ApiFilterAttribute> IApiContainerProvider.GlobalFilters => GlobalFilters;

        IEnumerable<DataValidationAttribute> IApiContainerProvider.GlobalValidations => GlobalValidations;

        IEnumerable<DataModificationAttribute> IApiContainerProvider.GlobalModifications => GlobalModifications;

        /// <summary>
        /// 创建返回值更新器
        /// </summary>
        /// <returns></returns>
        public virtual IResultUpdater CreateResultUpdater() => new ResultProvider();


        private static IEnumerable<Type> GetTypes()
        {
            var tempDomain = AppDomain.CreateDomain("UIF");

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
                    catch (Exception ex)
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

        private static readonly Regex _validChars = new Regex("^[a-z][0-9a-z_]+$", RegexOptions.Compiled|RegexOptions.IgnoreCase);
    }
}
