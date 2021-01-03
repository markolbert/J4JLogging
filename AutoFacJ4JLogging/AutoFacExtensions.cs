using System;
using Autofac;
using Serilog;

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterJ4JLogging( this ContainerBuilder builder, IJ4JLoggerConfiguration config )
        {
            builder.Register(c=>
                {
                    if( config.Channels == null )
                        throw new NullReferenceException( $"{nameof(config.Channels)} is not defined" );

                    var retVal = config.CreateBaseLogger();

                    if( retVal != null )
                        return retVal;

                    throw new NullReferenceException( $"Could not create ILogger from IJ4JLoggerConfiguration" );
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }

        public static ContainerBuilder RegisterJ4JLogging<TJ4JLogger>(
            this ContainerBuilder builder,
            IChannelFactory channelFactory )
        where TJ4JLogger : IJ4JLoggerConfiguration, new()
        {
            builder.Register( c =>
                {
                    var retVal = channelFactory.GetLoggerConfiguration<TJ4JLogger>();

                    if( retVal == null )
                        throw new NullReferenceException(
                            $"Could not extract an instance of {typeof(TJ4JLogger)} from IConfigurationRoot" );

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
