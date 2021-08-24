using Serilog;
using Serilog.Events;
using Twilio;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static J4JLoggerConfiguration IncludeSendToTwilio(
            this J4JLoggerConfiguration loggerConfig,
            TwilioConfiguration configValues,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
            string? outputTemplate = null )
        {
            if( !configValues.IsValid )
                return loggerConfig;

            outputTemplate ??= J4JLoggerConfiguration.GetOutputTemplate();

            var sink = new TwilioSink( configValues.FromNumber!, configValues.Recipients!, outputTemplate );

            try
            {
                TwilioClient.Init(configValues.AccountSID!, configValues.AccountToken!);
                sink.ClientConfigured = true;
            }
            catch
            {
                sink.ClientConfigured = false;
            }

            loggerConfig.SerilogConfiguration.WriteTo
                .Logger(lc =>
                    lc.Filter
                        .ByIncludingOnly("SendToSms")
                        .WriteTo.Sink(sink, restrictedToMinimumLevel)
                );

            loggerConfig.AddEnricher<SmsEnricher>();

            return loggerConfig;
        }
    }
}