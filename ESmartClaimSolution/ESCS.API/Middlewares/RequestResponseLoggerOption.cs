using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESCS.API.Middlewares
{
    public class RequestResponseLoggerOption
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public string DateTimeFormat { get; set; }
    }
}
