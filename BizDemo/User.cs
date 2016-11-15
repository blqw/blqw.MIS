using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.SIF;

namespace BizDemo
{
    [Api(Container = "iis", SettingString = "Route={class}/{action}")]
    [Api(Container = "wcf", SettingString = "Por")]
    public class User
    {
        [Api(Container = "iis", SettingString = "Route={id};HttpMethod=get")]
        public object Get(int id)
        {
            return new { id = 1, created_time = DateTime.Now };
        }
    }
}
