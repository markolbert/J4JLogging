using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autofac;
using Autofac.Configuration;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

#pragma warning disable 8618

namespace J4JLogger.Examples
{
    // shows how to use the J4JLogger with a configuration file whose structure
    // is derived from J4JLoggerConfiguration (i.e., the configuration class is a child
    // of J4JLoggerConfiguration).
    class Program
    {
        private static IServiceProvider _svcProvider;

        static void Main(string[] args)
        {
            InitializeServiceProvider();

            var logger = _svcProvider.GetRequiredService<IJ4JLogger>();
            logger.SetLoggedType<Program>();

            logger.Information("This is an Informational logging message");
        }

        private static void InitializeServiceProvider()
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile( Path.Combine( Environment.CurrentDirectory, "logConfig.json" ) )
                .Build();

            var configModule = new ConfigurationModule( config );

            var builder = new ContainerBuilder();

            builder.RegisterModule( configModule );

            builder.RegisterType<J4JLoggerConfiguration>()
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.Register( c =>
                {
                    var channelConfig = c.Resolve<ChannelConfiguration>();

                    return new ConsoleChannel( c.Resolve<IJ4JLoggerConfiguration>(), channelConfig.Console );
                })
                .As<ILogChannel>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var channelConfig = c.Resolve<ChannelConfiguration>();

                    return new DebugChannel(c.Resolve<IJ4JLoggerConfiguration>(), channelConfig.Debug);
                })
                .As<ILogChannel>()
                .SingleInstance();

            builder.Register(c =>
                {
                    var channelConfig = c.Resolve<ChannelConfiguration>();

                    return new FileChannel(c.Resolve<IJ4JLoggerConfiguration>(), channelConfig.File);
                })
                .As<ILogChannel>()
                .SingleInstance();

            builder.Register( c => config.GetSection( "Channels" ).Get<ChannelConfiguration>() )
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<LogChannels>()
                .AsSelf()
                .SingleInstance();

            builder.Register( c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
                    var channelSet = c.Resolve<LogChannels>();

                    return loggerConfig.CreateBaseLogger( channelSet );
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JSoftware.Logging.J4JLogger>()
                .As<IJ4JLogger>();
            
            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
