using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Session
{
    /// <summary>
    /// 用于表示Api类支持 Session 接口
    /// </summary>
    public interface ISupportSession
    {
        ISession Session { get; set; }
    }
}
