using blqw.MIS;
using blqw.MIS.DataModification;
using blqw.MIS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.MIS.Events;

namespace BizDemo
{
    public class Global : ApiGlobal
    {
        public override void Initialization()
        {
            base.Validations.Add(new NoScriptAttribute());
        }

        protected override void OnBeginRequest(ApiEventArgs e)
        {
            base.OnBeginRequest(e);
        }
    }
}
