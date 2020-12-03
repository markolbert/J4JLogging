﻿using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            var builder = new ContainerBuilder();

            var channelConfig = config.GetSection( "Channels" ).Get<ChannelConfiguration>();

            builder.Register( c =>
                {
                    var retVal = config.GetSection( "Logging" ).Get<J4JLoggerConfiguration<ChannelConfiguration>>();

                    retVal.Channels = channelConfig;

                    return retVal;
                } )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.RegisterJ4JLogging();

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }
    }
}
