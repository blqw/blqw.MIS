using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.UnitTest
{
    /// <summary>
    /// 一个简易的表达式树解析器
    /// </summary>
    static class ExpressionParser
    {
        public static Request Parse(Expression body)
        {
            var expr = body as MethodCallExpression
                    ?? (body as LambdaExpression)?.Body as MethodCallExpression
                    ?? throw new InvalidOperationException("无法解析测试表达式");

            var instance = GetValue(expr.Object);
            var arguments = new List<ApiArgument>();
            var properties = new List<ApiProperty>();
            if (expr.Object is MemberInitExpression x)
            {
                foreach (var b in x.Bindings)
                {
                    if (b is MemberAssignment ma)
                    {
                        if (b.Member is FieldInfo f)
                        {
                            f.SetValue(instance, GetValue(ma.Expression));
                        }
                        else if (b.Member.IsDefined(typeof(ApiPropertyAttribute)))
                        {
                            properties.Add(new ApiProperty((PropertyInfo)b.Member, GetValue(ma.Expression)));
                        }
                        else if (b.Member is PropertyInfo p)
                        {
                            p.SetValue(instance, GetValue(ma.Expression));
                        }
                    }
                }
            }
            foreach (var parameter in expr.Method.GetParameters())
            {
                arguments.Add(new ApiArgument(parameter, GetValue(expr.Arguments[parameter.Position])));
            }
            return new Request()
            {
                Method = expr.Method,
                Instance = instance,
                Arguments = arguments.AsReadOnly(),
                Properties = properties.AsReadOnly(),
                ActualRequest = body,
            };
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
