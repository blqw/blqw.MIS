using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS
{
    public static class UnitTest
    {
        static ApiContainer _container = new ApiContainer("UnitTest", new UnitTestProvider());

        public static UnitTestResult Call(Expression<Action> testCase)
        {
            var result = ExpressionParser.Parse(testCase);
            var api = _container.ApiCollection.Apis.FirstOrDefault(a => a.Method == result.Method);
            if (api == null)
            {
                throw new InvalidOperationException("无效api方法");
            }
            var context = api.Invoke(new DataProvider(result));
            if (context.IsError)
            {
                return new UnitTestResult(context.Exception);
            }
            return new UnitTestResult();
        }

        public static UnitTestResult<T> Call<T>(Expression<Func<T>> testCase)
        {
            var result = ExpressionParser.Parse(testCase);
            var api = _container.ApiCollection.Apis.FirstOrDefault(a => a.Method == result.Method);
            if (api == null)
            {
                throw new InvalidOperationException("无效api方法");
            }
            var context = api.Invoke(new DataProvider(result));
            if (context.IsError)
            {
                return new UnitTestResult<T>(context.Exception);
            }
            return new UnitTestResult<T>((T)context.Result);
        }
    }
}
