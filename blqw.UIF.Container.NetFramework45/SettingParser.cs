using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.UIF.Services;

namespace blqw.UIF.NetFramework45
{
    public class SettingParser : IApiSettingParser
    {
        public void Dispose()
        { 
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name => nameof(SettingParser);

        /// <summary>
        /// 服务属性集
        /// </summary>
        public IDictionary<string, object> Data { get; } = new NameDictionary();

        /// <summary>
        /// 使用时是否必须克隆出新对象
        /// </summary>
        public bool RequireClone => false;

        /// <summary>
        /// 克隆当前对象,当<see cref="IService.RequireClone"/>为true时,每次使用服务将先调用该方法获取新对象
        /// </summary>
        /// <returns></returns>
        public IService Clone() => new SettingParser();
        
        /// <summary>
        /// 解析设置
        /// </summary>
        /// <param name="settings"> 待解析的字符串 </param>
        /// <returns></returns>
        public ParseResult Parse(IEnumerable<string> settings)
        {
            if (settings == null) return new ParseResult(new NameDictionary());
            try
            {
                var items = new NameDictionary();
                var builder = new DbConnectionStringBuilder();
                foreach (var setting in settings)
                {
                    builder.ConnectionString = setting;
                    foreach (string key in builder.Keys)
                    {
                        items[key] = builder[key];
                    }
                }
                return new ParseResult(items);
            }
            catch (Exception ex)
            {
                return new ParseResult(ex);
            }
        }
    }
}
