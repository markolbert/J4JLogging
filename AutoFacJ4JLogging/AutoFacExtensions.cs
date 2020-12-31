using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterJ4JLogging( this ContainerBuilder builder )
        {
            builder.Register(c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                    return loggerConfig.CreateBaseLogger()!;
                })
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }

        public static ContainerBuilder RegisterJ4JLogging(
            this ContainerBuilder builder, 
            IConfigurationRoot config, 
            string? logKey = null, 
            AvailableChannels channels = AvailableChannels.All,
            EventElements defaultElements = EventElements.All,
            LogEventLevel minLevel = LogEventLevel.Verbose,
            string outputTemplate = ChannelConfig.DefaultOutputTemplate,
            TwilioConfig? twilioConfig = null )
        {
            if( twilioConfig == null )
                channels = channels & ~AvailableChannels.Twilio;
            else channels |= AvailableChannels.Twilio;

            builder.Register(c =>
                {
                    var retVal = string.IsNullOrEmpty( logKey )
                        ? config.Get<J4JLoggerConfiguration<DefaultLogChannels>>()
                        : config.GetSection( logKey ).Get<J4JLoggerConfiguration<DefaultLogChannels>>();

                    retVal ??= new J4JLoggerConfiguration<DefaultLogChannels>();

                    retVal.Channels.ActiveChannels = channels;
                    retVal.Channels.EventElements = defaultElements;
                    retVal.Channels.MinimumLevel = minLevel;
                    retVal.Channels.OutputTemplate = outputTemplate;
                    retVal.Channels.Twilio = twilioConfig;

                    return retVal;
                })
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();

                    return loggerConfig.CreateBaseLogger()!;
                })
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }
    }
}
