using System.ComponentModel;
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
            IncludeLastEvent inclLastEvent = IncludeLastEvent.DoNotOverride,
            TwilioConfig? twilioConfig = null )
        {
            builder.Register(c =>
                {
                    var retVal = ( string.IsNullOrEmpty( logKey )
                                     ? config.Get<J4JLoggerConfiguration<DefaultLogChannels>>()
                                     : config.GetSection( logKey ).Get<J4JLoggerConfiguration<DefaultLogChannels>>() );

                    if( retVal == null )
                    {
                        retVal = new J4JLoggerConfiguration<DefaultLogChannels>();

                        retVal.Channels.IncludeLastEvent = inclLastEvent switch
                        {
                            IncludeLastEvent.FalseAlways => false,
                            IncludeLastEvent.FalseForDefaultLogChannels => false,
                            IncludeLastEvent.TrueAlways => true,
                            IncludeLastEvent.TrueForDefaultLogChannels => true,
                            _ => retVal.Channels.IncludeLastEvent
                        };
                    }
                    else retVal.Channels.IncludeLastEvent = inclLastEvent switch
                        {
                            IncludeLastEvent.FalseAlways => false,
                            IncludeLastEvent.TrueAlways => true,
                            _ => retVal.Channels.IncludeLastEvent
                        };

                    if ( twilioConfig == null ) 
                        return retVal;
                    
                    retVal.Channels.ActiveChannels |= AvailableChannels.Twilio;
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
