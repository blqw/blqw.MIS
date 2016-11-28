using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw.SIF.Validation
{
    /// <summary>
    /// 验证器
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// 验证方法参数是否合法
        /// </summary>
        /// <param name="method">待验证参数的方法</param>
        /// <param name="args">需要验证的参数</param>
        /// <param name="lazy">懒惰模式,存在第一个不合法的参数立即返回,为false则返回所有不合法的验证结果</param>
        /// <returns>返回null验证通过</returns>
        public static Exception IsValid(MethodInfo method, IDictionary<string, object> args, bool lazy)
        {
            return null;
        }
    }
}
