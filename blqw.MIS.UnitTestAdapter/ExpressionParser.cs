using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    static class ExpressionParser
    {
        public static ParseResult Parse(Expression body)
        {
            var expr = body as MethodCallExpression
                    ?? (body as LambdaExpression)?.Body as MethodCallExpression
                    ?? throw new InvalidOperationException("无法解析测试表达式");
            var result = new ParseResult()
            {
                Instance = GetValue(expr.Object),
                Method = expr.Method,
                Parameters = new object[expr.Method.GetParameters().Length],
                Properties = new Dictionary<MemberInfo, object>()
            };
            if (expr.Object is MemberInitExpression x)
            {
                foreach (var b in x.Bindings)
                {
                    if (b is MemberAssignment ma)
                    {
                        if (b.Member is FieldInfo f)
                        {
                            f.SetValue(result.Instance, GetValue(ma.Expression));
                        }
                        else if (b.Member.IsDefined(typeof(ApiPropertyAttribute)))
                        {
                            result.Properties.Add(b.Member, GetValue(ma.Expression));
                        }
                        else if (b.Member is PropertyInfo p)
                        {
                            p.SetValue(result.Instance, GetValue(ma.Expression));
                        }
                    }
                }
            }
            for (int i = 0; i < result.Parameters.Count; i++)
            {
                result.Parameters[i] = GetValue(expr.Arguments[i]);
            }
            return result;
        }

        private static object GetValue(Expression expr)
        {
            switch (expr)
            {
                case ConstantExpression e:
                    return e.Value;
                case NewExpression e:
                    return e.Constructor.Invoke(e.Arguments.Select(a => GetValue(a)).ToArray());
                case MemberInitExpression e:
                    return GetValue(e.NewExpression);
                default:
                    break;
            }
            throw new InvalidOperationException("无法解析测试表达式");
        }
    }
}
