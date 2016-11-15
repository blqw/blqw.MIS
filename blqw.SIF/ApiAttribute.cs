using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw.SIF
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ApiAttribute : Attribute
    {
        public string Container { get; set; }
        public string SettingString { get; set; }

        private Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string name]
        {
            get
            {
                string value;
                return _values.TryGetValue(name, out value) ? value : null;
            }
        }
    }
}
