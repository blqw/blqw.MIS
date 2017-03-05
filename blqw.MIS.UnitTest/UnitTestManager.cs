using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.UnitTest
{
    public static class UnitTestManager
    {
        private static readonly Scheduler _scheduler = new Scheduler(new ServiceEntry(new ApiContainer("UnitTest", ExportedTypes.Enumerable())));

        public static UnitTestResult Call(Expression<Action> testCase)
        {
            try
            {
                var request = ExpressionParser.Parse(testCase);
                var result = _scheduler.Execute(request);
                if (result is Exception ex)
                {
                    return new UnitTestResult(ex);
                }
                return new UnitTestResult();
            }
            catch (Exception e)
            {
                return new UnitTestResult(e);
            }
        }

        public static UnitTestResult<T> Call<T>(Expression<Func<T>> testCase)
        {
            try
            {
                var request = ExpressionParser.Parse(testCase);
                var result = _scheduler.Execute(request);
                if (result is Exception ex)
                {
                    return new UnitTestResult<T>(ex);
                }
                return new UnitTestResult<T>((T)result);
            }
            catch (Exception e)
            {
                return new UnitTestResult<T>(e);
            }
        }

        public static UnitTestResult<T> Call<T>(Expression<Func<Task<T>>> testCase)
        {
            try
            {
                var request = ExpressionParser.Parse(testCase);
                var result = _scheduler.Execute(request);
                if (result is Exception ex)
                {
                    return new UnitTestResult<T>(ex);
                }
                return new UnitTestResult<T>((T)result);
            }
            catch (Exception e)
            {
                return new UnitTestResult<T>(e);
            }
        }

        public static UnitTestResult Call(Expression<Func<Task>> testCase)
        {
            try
            {
                var request = ExpressionParser.Parse(testCase);
                var result = _scheduler.Execute(request);
                if (result is Exception ex)
                {
                    return new UnitTestResult(ex);
                }
                return new UnitTestResult();
            }
            catch (Exception e)
            {
                return new UnitTestResult(e);
            }
        }
    }
}
