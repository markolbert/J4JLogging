﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AutoFacJ4JLogging
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder AddJ4JLogging<TConfig>( 
            this ContainerBuilder builder, 
            string configFilePath, 
            params Type[] channelTypes)
            where TConfig : class, IJ4JLoggerConfiguration
        {
            var channels = GetChannels( channelTypes );

            builder.RegisterChannels( channels )
                .RegisterLogger()
                .Register( c =>
                {
                    var configBuilder = new J4JLoggerConfigurationJsonBuilder();

                    foreach( var kvp in channels )
                    {
                        configBuilder.AddChannel( kvp.Value );
                    }

                    configBuilder.FromFile( configFilePath );

                    return configBuilder.Build<TConfig>();
                } )
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.AddJ4JLogging();

            return builder;
        }

        public static ContainerBuilder AddJ4JLogging<TConfig>( 
            this ContainerBuilder builder, 
            IConfigurationRoot configRoot, 
            string loggerSection, 
            params Type[] channelTypes )
        where TConfig : class, IJ4JLoggerConfiguration
        {
            builder.Register( c =>
                {
                    var loggerBuilder = new J4JLoggerConfigurationRootBuilder();

                    foreach( var channelType in channelTypes )
                    {
                        loggerBuilder.AddChannel( channelType );
                    }

                    return loggerBuilder.Build<TConfig>( configRoot, loggerSection );
                } )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            builder.AddJ4JLogging();

            return builder;
        }

        private static ContainerBuilder AddJ4JLogging(this ContainerBuilder builder)
        {
            builder.Register(c => c.Resolve<IJ4JLoggerConfiguration>().CreateLogger())
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }

        private static ContainerBuilder RegisterChannels( this ContainerBuilder builder, Dictionary<string, Type> channels )
        {
            foreach (var kvp in channels)
            {
                builder.RegisterType(kvp.Value)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            return builder;
        }

        public static ContainerBuilder RegisterLogger( this ContainerBuilder builder )
        {
            builder.Register( c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
                    return loggerConfig.CreateLogger();
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }

        private static Dictionary<string, Type> GetChannels( Type[] channelTypes )
        {
            var retVal = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var channelType in channelTypes)
            {
                if (!(typeof(ILogChannel).IsAssignableFrom(channelType)))
                    continue;

                var attr = channelType.GetCustomAttributes(typeof(ChannelAttribute), false)
                    .Cast<ChannelAttribute>()
                    .FirstOrDefault();

                if (attr == null)
                    continue;

                if (retVal.ContainsKey(attr.ChannelID)) retVal[attr.ChannelID] = channelType;
                else retVal.Add(attr.ChannelID, channelType);
            }

            return retVal;
        }
    }
}
