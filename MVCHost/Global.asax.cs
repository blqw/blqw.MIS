using blqw.MIS.MVCAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace MVCHost
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add("mis", new MISRoute());
        }

    }
}