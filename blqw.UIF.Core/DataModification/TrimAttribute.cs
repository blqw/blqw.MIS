using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.DataModification
{
    /// <summary>
    /// 字符串去空格
    /// </summary>
    public class TrimAttribute: DataModificationAttribute
    {
        public bool TrimEnd { get; set; } = true;

        public bool TrimStart { get; set; } = true;

        public override void Modifies(ref object arg)
        {
            var str = arg as string;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            if (TrimStart && char.IsWhiteSpace(str[0]))
            {
                if (TrimEnd && char.IsWhiteSpace(str[str.Length - 1]))
                {
                    arg = str.Trim();
                }
                else
                {
                    arg = str.TrimStart();
                }
                return;
            }
            if (TrimEnd && char.IsWhiteSpace(str[str.Length - 1]))
            {
                arg = str.TrimEnd();
            }
        }

        public override bool Match(DataModificationAttribute attribute)
            => base.Match(attribute) || attribute.GetType() == typeof(NoTrimAttribute);
    }
}
