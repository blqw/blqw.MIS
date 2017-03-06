using blqw.MIS;
using System;
using System.Threading.Tasks;

namespace BizDemo
{
    public class User
    {
        [ApiProperty]
        public int X { get; set; }

        [Api]
        public object Test()
        {
            return "hello mis";
        }


        [Api]
        public async Task<string> TestAsync()
        {
            return "hello mis";
        }
    }
}
