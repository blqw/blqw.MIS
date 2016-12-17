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

namespace blqw.UIF.NetFramework45
{
    /// <summary>
    /// 基于 .NET Framework 4.5 实现的容器提供程序
    /// </summary>
    public class ApiContainerProvider : IApiContainerProvider
    {
        protected ApiContainerProvider(string containerId)
            : this(containerId, GetTypes())
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

    }
}
