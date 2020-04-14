using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using J4JSoftware.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AutoFacJ4JLogging
{
    public static class AutoFacExtensions
    {
        public static ContainerBuilder AddJ4JLoggingFromFile<TConfig>( this ContainerBuilder builder, string configFilePath, params Type[] channelTypes)
            where TConfig : class, IJ4JLoggerConfiguration
        {
            var channels = DoCoreConfiguration( builder, channelTypes );

            builder.Register( c =>
                {
                    var configBuilder = new J4JLoggerConfigurationBuilder();

                    foreach( var kvp in channels )
                    {
                        configBuilder.AddChannel( kvp.Value );
                    }

                    configBuilder.FromFile( configFilePath );

                    return configBuilder.Build<TConfig>();
                })
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddJ4JLoggingFromJsonText<TConfig>(this ContainerBuilder builder, string jsonText, params Type[] channelTypes)
            where TConfig : class, IJ4JLoggerConfiguration
        {
            var channels = DoCoreConfiguration(builder, channelTypes);

            builder.Register(c =>
                {
                    var configBuilder = new J4JLoggerConfigurationBuilder();

                    foreach (var kvp in channels)
                    {
                        configBuilder.AddChannel(kvp.Value);
                    }

                    configBuilder.FromJson(jsonText);

                    return configBuilder.Build<TConfig>();
                })
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            return builder;
        }

        public static ContainerBuilder AddJ4JLogging(this ContainerBuilder builder, IJ4JLoggerConfiguration config, params Type[] channelTypes)
        {
            DoCoreConfiguration(builder, channelTypes);

            builder.Register( c => config )
                .As<IJ4JLoggerConfiguration>()
                .SingleInstance();

            return builder;
        }

        private static Dictionary<string, Type> DoCoreConfiguration( ContainerBuilder builder, Type[] channelTypes )
        {
            var channels = GetChannels( channelTypes );

            foreach( var kvp in channels )
            {
                builder.RegisterType( kvp.Value )
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            builder.Register( c =>
                {
                    var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
                    return loggerConfig.CreateLogger();
                } )
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLoggerFactory>()
                .As<IJ4JLoggerFactory>()
                .SingleInstance();

            return channels;
        }

        private static Dictionary<string, Type> GetChannels( Type[] channelTypes )
        {
            var retVal = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var channelType in channelTypes)
            {
                if (!(typeof(IChannelConfiguration).IsAssignableFrom(channelType)))
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
