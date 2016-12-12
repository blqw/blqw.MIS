using blqw.SIF;
using blqw.SIF.DataModification;
using blqw.SIF.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizDemo
{
    public class Global : ApiGlobal
    {
        public override void Initialization()
        {
            base.Validations.Add(new NoScriptAttribute());
        }
    }
}
