using blqw.IOC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw.SIF.Owin
{
    static class Logger
    {
        public static LoggerSource Source { get; } = new LoggerSource("blqw.SIF.Owin", SourceLevels.Error);
    }
}
