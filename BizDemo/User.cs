using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF;

namespace BizDemo
{
    [SI(Container = "iis", SettingString = "Route={class}/{action}")]
    [SI(Container = "wcf", SettingString = "Por")]
    public class User
    {
        [SI(Container = "iis", SettingString = "Route={id};HttpMethod=get")]
        public object Get(int id)
        {
            return new { id = 1, created_time = DateTime.Now };
        }
    }
}
