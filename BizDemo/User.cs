using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF;

namespace BizDemo
{
    public class User
    {
        [Api]
        public object Get(int id)
        {
            
            return new { id = id, created_time = DateTime.Now };
        }
    }
}
