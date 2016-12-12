using blqw.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.UIF.Owin
{
    static class Logger
    {
        public static LoggerSource Source { get; } = new LoggerSource("blqw.UIF.Owin", SourceLevels.Error);
    }
}
