using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.MVC
{
    /// <summary>
    /// <seealso cref="StringBuilder"/>实例池
    /// </summary>
    internal static class StringBuilderPool
    {
        /// <summary>
        /// 取出一个<seealso cref="StringBuilder"/>实例
        /// </summary>
        /// <returns></returns>
        public static StringBuilder GetOut()
        {
            return new StringBuilder();
        }

        /// <summary>
        /// 将<seealso cref="StringBuilder"/>实例还入池中
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string Return(StringBuilder builder)
        {
            try
            {
                return builder.ToString();
            }
            finally
            {

            }
        }
    }
}
