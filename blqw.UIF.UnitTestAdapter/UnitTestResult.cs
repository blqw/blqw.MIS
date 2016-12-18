using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF
{
    public class UnitTestResult
    {
        public UnitTestResult()
        {
            Succeed = true;
        }

        public UnitTestResult(Exception ex)
        {
            Exception = ex;
            Succeed = false;
        }

        public bool Succeed { get; }

        public Exception Exception { get; }
    }

    public class UnitTestResult<T>
    {
        public UnitTestResult(T result)
        {
            Succeed = true;
            Result = result;
        }

        public UnitTestResult(Exception ex)
        {
            Exception = ex;
            Succeed = false;
        }

        public bool Succeed { get; }

        public Exception Exception { get; }

        public T Result { get; }
    }
}
