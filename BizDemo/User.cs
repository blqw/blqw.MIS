using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF;
using blqw.SIF.DataModification;
using blqw.SIF.Validation;

namespace BizDemo
{
    public class User
    {
        [Api]
        public object Get([Positive]int id, [Trim]string name)
        {
            return new { id = id, name = name, created_time = DateTime.Now };
        }
    }
}
