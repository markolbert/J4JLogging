﻿using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace J4JLoggingTests
{
    public class CompositionRoot
    {
        private readonly IServiceProvider _svcProvider;

        public CompositionRoot( string configPath, string loggerKey )
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddJsonFile(Path.Combine(Environment.CurrentDirectory, configPath))
                .Build();
            
            var builder = new ContainerBuilder();

            //var channelConfig = config.GetSection(channelsKey).Get<ChannelConfiguration>();

            //builder.Register(c =>
            //    {
            //        var retVal = config.GetSection( loggerKey ).Get<J4JLoggerConfiguration<ChannelConfiguration>>();

            //        retVal.Channels = channelConfig;

            //        return retVal;
            //    } )
            //    .As<IJ4JLoggerConfiguration>()
            //    .SingleInstance();

            builder.RegisterJ4JLogging( config,
                loggerKey,
                AvailableChannels.All | AvailableChannels.LastEvent,
                twilioConfig : new TwilioTestConfig() );

            builder.Register( c =>
                {
                    var logConfig = (IJ4JLoggerConfiguration<DefaultLogChannels>) c.Resolve<IJ4JLoggerConfiguration>();
                    return logConfig.Channels.LastEvent;
                } )
                .AsSelf()
                .SingleInstance();

            _svcProvider = new AutofacServiceProvider( builder.Build() );
        }

        public IJ4JLogger J4JLogger => _svcProvider.GetRequiredService<IJ4JLogger>();
        public LastEventConfig LastEventConfig => _svcProvider.GetRequiredService<LastEventConfig>();
    }
}
