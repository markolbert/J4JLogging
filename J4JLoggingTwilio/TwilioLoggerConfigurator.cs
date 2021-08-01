using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class TwilioLoggerConfigurator : LoggerConfigurator
    {
        public TwilioLoggerConfigurator(
            Func<J4JLogger, Type, IChannel?> channelFactory
        )
            : base( channelFactory )
        {
            ScanAssemblyForChannels();
        }
    }
}
