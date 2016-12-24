﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS;
using blqw.MIS.DataModification;
using blqw.MIS.Validation;
using blqw.MIS.Session;

namespace BizDemo
{
    public class User //: ISupportSession
    {
        [ApiProperty]
        [Range(1, 1000)]
        public int X { get; set; }

        //public ISession Session { get; set; }

        [Api]
        public void SetSession(string name, string value, ISession Session)
        {
            
            Session[name] = value;
        }

        [Api]
        public object GetSession(string name, ISession Session)
        {
            return Session[name];
        }

        [Api]
        public object Get([Range(1, 2)]long id, string name, [Checked(1, 100)]int page = 1)
        {
            return new { id = id, name = name, created_time = DateTime.Now, page };
        }
    }
}
