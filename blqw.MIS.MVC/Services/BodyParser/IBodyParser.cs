using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.MIS.MVC.Services
{
    [InheritedExport]
    interface IBodyParser
    {
        bool Match(string mineType);

        IEnumerable<ApiArgument> Parse(Request request);
    }
}
