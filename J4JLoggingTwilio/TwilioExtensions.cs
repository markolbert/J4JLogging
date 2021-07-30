using System;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static TwilioChannel AddTwilio( this J4JLogger logger, ITwilioParameters? parameters = null)
        {
            var retVal = new TwilioChannel( logger );

            if( parameters != null )
                retVal.ApplySettings( parameters );

            logger.Channels.Add( retVal );

            return retVal;
        }

        public static TwilioChannel ApplySettings( this TwilioChannel channel, ITwilioParameters values )
        {
            channel.Parameters.MinimumLevel = values.MinimumLevel;
            channel.Parameters.AccountSID = values.AccountSID;
            channel.Parameters.AccountToken = values.AccountToken;
            channel.Parameters.FromNumber = values.FromNumber;
            channel.Parameters.Recipients = values.Recipients;

            return channel;
        }

        public static TwilioParameters SetCredentials( this TwilioParameters container, string sid, string token )
        {
            container.AccountSID = sid;
            container.AccountToken = token;

            return container;
        }

        public static TwilioParameters SetAccountSID( this TwilioParameters container, string acctSID )
        {
            container.AccountSID = acctSID;
            return container;
        }

        public static TwilioParameters SetAccountToken(this TwilioParameters container, string acctToken)
        {
            container.AccountToken = acctToken;
            return container;
        }

        public static TwilioParameters SetFromNumber(this TwilioParameters container, string fromNumber)
        {
            container.FromNumber = fromNumber;
            return container;
        }
    }
}