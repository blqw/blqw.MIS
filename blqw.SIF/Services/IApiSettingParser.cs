using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF.Services
{
    /// <summary>
    /// Api设置解释器
    /// </summary>
    public interface IApiSettingParser : IService
    {
        bool TryParse(string apiSettingString, out IDictionary<string, object> settings);
    }
}
