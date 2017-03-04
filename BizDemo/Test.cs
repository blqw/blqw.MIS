using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS;

namespace BizDemo
{
    public class Test
    {
        /// <summary>
        /// 返回 Hello
        /// </summary>
        /// <returns></returns>
        [Api]
        public string Hello()
            => "Hello";


        /// <summary>
        /// 返回 Hello
        /// </summary>
        [Api]
        public Task<string> HelloAsync()
            => Task.FromResult("Hello");

        [ApiProperty]
        public string Name { get; set; }

        [Api]
        public void CheckProp(string name)
        {
            if (name != Name)
            {
                throw new Exception();
            }
        }

        [Api]
        public async Task CheckPropAsync(string name)
        {
            if (name != Name)
            {
                throw new Exception();
            }
        }

        public void NotApi()
        {
            
        }
    }
}
