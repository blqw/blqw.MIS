using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Services;

namespace blqw.MIS.NetFramework45
{
    public class Convert3Proxy : IConverter
    {
        public Convert3Proxy()
            : this(nameof(Convert3Proxy))
        {

        }
        protected Convert3Proxy(string name)
        {
            Name = name;
            Data = new NameDictionary();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; }

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public virtual bool RequireClone { get; } = false;

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public virtual IService Clone() => new Convert3Proxy(Name);

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <param name="value"> 待转换类型的值 </param>
        /// <param name="conversionType">  要返回的对象的类型。 </param>
        /// <returns></returns>
        /// <remarks>此接口不应抛出异常</remarks>
        public ConvertResult ChangeType(object value, Type conversionType)
        {
            try
            {
                return new ConvertResult(value.ChangeType(conversionType));
            }
            catch (Exception ex)
            {
                return new ConvertResult(ex);
            }
        }
    }
}
