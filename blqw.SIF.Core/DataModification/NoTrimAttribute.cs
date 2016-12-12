using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.DataModification
{
    /// <summary>
    /// 字符串去空格
    /// </summary>
    public class NoTrimAttribute : DataModificationAttribute
    {
        public override void Modifies(ref object arg)
        {
            
        }

        public override bool Match(DataModificationAttribute attribute)
            => base.Match(attribute) || attribute.GetType() == typeof(TrimAttribute);
    }
}
