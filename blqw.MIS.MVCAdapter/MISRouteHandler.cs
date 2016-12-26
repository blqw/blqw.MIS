using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Routing;

namespace blqw.MIS.MVCAdapter
{
    public partial class MISRouteHandler : IRouteHandler
    {
        private readonly MISHttpHandler _httpHandler = new MISHttpHandler();
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
            => _httpHandler;
    }
}
