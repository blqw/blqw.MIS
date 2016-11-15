using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF.Descriptor
{
    /// <summary>
    /// 对于一个接口的描述
    /// </summary>
    public sealed class ApiDescriptor
    {
        /// <summary>
        /// 接口方法
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// 接口类
        /// </summary>
        public Type Type { get; }



    }
}
