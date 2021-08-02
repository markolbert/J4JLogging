using System;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static TwilioChannel ConfigureFileChannel(
            this TwilioChannel channel,
            TwilioConfiguration? configValues = null)
        {
            if (configValues == null)
                return channel;

            channel.ConfigureChannel( configValues );

            channel.AccountToken = configValues.AccountToken;
            channel.AccountSID = configValues.AccountSID;
            channel.FromNumber = configValues.FromNumber;
            channel.Recipients = configValues.Recipients;

            return channel;
        }

        public static TwilioChannel AddTwilio( this J4JLogger logger, TwilioConfiguration? configValues = null )
        {
            var retVal = new TwilioChannel();
            retVal.SetAssociatedLogger( logger );

            logger.Channels.Add( retVal );

            if( configValues == null )
                return retVal;

            if( configValues.RequireNewLine.HasValue )
                retVal.RequireNewLine = configValues.RequireNewLine.Value;

            if( configValues.MinimumLevel.HasValue )
                retVal.MinimumLevel = configValues.MinimumLevel.Value;

            if( configValues.IncludeSourcePath.HasValue )
                retVal.IncludeSourcePath = configValues.IncludeSourcePath.Value;

            if( configValues.OutputTemplate != null )
                retVal.OutputTemplate = configValues.OutputTemplate;

            retVal.SourceRootPath = configValues.SourceRootPath;

            retVal.AccountToken = configValues.AccountToken;
            retVal.AccountSID = configValues.AccountSID;
            retVal.FromNumber = configValues.FromNumber;
            retVal.Recipients = configValues.Recipients;

            return retVal;
        }
    }
}