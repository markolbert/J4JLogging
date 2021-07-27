using System;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static TwilioChannel AddTwilio(
            this J4JLogger logger,
            string accountSID,
            string accountToken,
            string fromNumber,
            LogEventLevel minLevel = LogEventLevel.Verbose )
        {
            var retVal = new TwilioChannel(logger);

            retVal.Parameters.MinimumLevel = minLevel;
            retVal.Parameters.AccountSID = accountSID;
            retVal.Parameters.AccountToken = accountToken;
            retVal.Parameters.FromNumber = fromNumber;

            logger.Channels.Add(retVal);

            return retVal;
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