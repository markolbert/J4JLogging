using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Twilio;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        //public static TwilioChannel ConfigureFileChannel(
        //    this TwilioChannel channel,
        //    TwilioConfiguration? configValues = null)
        //{
        //    if (configValues == null)
        //        return channel;

        //    channel.ConfigureChannel( configValues );

        //    channel.AccountToken = configValues.AccountToken;
        //    channel.AccountSID = configValues.AccountSID;
        //    channel.FromNumber = configValues.FromNumber;
        //    channel.Recipients = configValues.Recipients;

        //    return channel;
        //}

        public static J4JLogger IncludeSendToTwilio(
            this J4JLogger logger,
            TwilioConfiguration configValues,
            ITextFormatter? formatter = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose )
        {
            var sink = SinkExtensions.CreateSmsSink<TwilioSink>(
                configValues.FromNumber,
                configValues.Recipients,
                formatter);

            try
            {
                TwilioClient.Init(configValues.AccountSID, configValues.AccountToken);
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