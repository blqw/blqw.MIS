using blqw.MIS;
using System;

namespace BizDemo
{
    public class User //: ISupportSession
    {
        [ApiProperty]
        //[Range(1, 1000)]
        public int X { get; set; }

        [Api]
        public object Test()
        {
            return "hello mis";
        }

        //public ISession Session { get; set; }

        //[Api]
        //public void SetSession(string name, string value, ISession Session)
        //{
            
        //    Session[name] = value;
        //}

        //[Api]
        //public object GetSession(string name, ISession Session)
        //{
        //    return Session[name];Activator.CreateInstance<>()
        //}

        //[Api]
        //public object Get([Range(1, 2),Positive()]long id, string name, [Checked(1, 100)]int page = 1)
        //{
        //    return new { id = id, name = name, created_time = DateTime.Now, page };
        //}
    }
}
