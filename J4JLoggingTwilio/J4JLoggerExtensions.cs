using System;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class J4JLoggerExtensions
    {
        public static TwilioChannel AddTwilio(
            this J4JLogger logger,
            string accountSID,
            string accountToken,
            string fromNumber,
            LogEventLevel minLevel = LogEventLevel.Verbose )
        {
            var retVal = new TwilioChannel(logger);

            retVal.Parameters = retVal.Parameters == null
                ? new TwilioParameters( logger )
                {
                    MinimumLevel = minLevel,
                    AccountSID = accountSID,
                    AccountToken = accountToken,
                    FromNumber = fromNumber
                }
                : retVal.Parameters! with
                {
                    MinimumLevel = minLevel,
                    AccountSID = accountSID,
                    AccountToken = accountToken,
                    FromNumber = fromNumber
                };

            logger.Channels.Add(retVal);

            return retVal;
        }
    }
}