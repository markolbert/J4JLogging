using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using J4JSoftware.Logging;

namespace J4JLogger.Examples
{
    public class ChannelConfiguration
    {
        public ConsoleConfig Console { get; set; }
        public DebugConfig Debug { get; set; }
        public FileConfig File { get; set; }
    }
}
