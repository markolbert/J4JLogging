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
            Func<J4JLogger, string, IChannel?> channelFactory
        )
            : base( channelFactory )
        {
        }

        protected override bool ConfigureChannel( IChannel channel, ChannelConfiguration? config, out IChannel? result )
        {
            if( base.ConfigureChannel( channel, config, out result ) )
                return true;

            result = channel switch
            {
                TwilioChannel twilioChannel => twilioChannel.ConfigureFileChannel( (TwilioConfiguration?) config ),
                _ => null
            };

            return result != null;
        }
    }
}
