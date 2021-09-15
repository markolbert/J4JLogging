using System;
using Serilog;
using Serilog.Events;
using Twilio;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static J4JLoggerConfiguration AddTwilio(
            this J4JLoggerConfiguration loggerConfig,
            TwilioConfiguration configValues,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            string? outputTemplate = null )
        {
            if( !configValues.IsValid )
                throw new ArgumentException( "Twilio configuration values are invalid" );

            var sink = new TwilioSink( configValues.FromNumber!, 
                configValues.Recipients!,
                outputTemplate ?? loggerConfig.GetOutputTemplate() );

            try
            {
                TwilioClient.Init( configValues.AccountSID!, configValues.AccountToken! );
                sink.IsConfigured = true;

                loggerConfig.AddSmsSink( sink, restrictedToMinimumLevel );
            }
            catch
            {
                sink.IsConfigured = false;
            }

            return loggerConfig;
        }
    }
}