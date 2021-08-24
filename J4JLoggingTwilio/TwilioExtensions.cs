using Serilog;
using Serilog.Events;
using Twilio;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static J4JLogger IncludeSendToTwilio(
            this J4JLogger logger,
            TwilioConfiguration configValues,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose )
        {
            if( !configValues.IsValid )
                return logger;

            var sink = new TwilioSink( configValues.FromNumber!, configValues.Recipients!, logger.GetOutputTemplate() );

            try
            {
                TwilioClient.Init(configValues.AccountSID!, configValues.AccountToken!);
                sink.ClientConfigured = true;
            }
            catch
            {
                sink.ClientConfigured = false;
            }

            logger.LoggerConfiguration.WriteTo
                .Logger(lc =>
                    lc.Filter
                        .ByIncludingOnly("SendToSms")
                        .WriteTo.Sink(sink, restrictedToMinimumLevel)
                );

            logger.AddEnricher<SmsEnricher>();

            return logger;
        }
    }
}