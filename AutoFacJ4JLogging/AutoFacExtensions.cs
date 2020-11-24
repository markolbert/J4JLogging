using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace J4JSoftware.Logging
{
    public static class AutofacExtensions
    {
        // Configures the J4JLogging system based on a JSON configuration file whose structure either
        // matches or is derived from J4JLoggerConfiguration.
        //
        // channelTypes is a collection of Types implementing ILogChannel and decorated with a ChannelAttribute
        // defining the name/ID of the channel. Channel names/IDs should be unique. Type not fulfilling
        // the ILogChannel/ChannelAttribute constraints will be ignored. If duplicate Channel names/IDs are
        // specified only the last instance will be retained and used by the library.
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

            //builder.AddJ4JLogging();

            return builder;
        }

        // Configures the J4JLogging system based on an IConfigurationRoot object. This is targeted at
        // supporting configuration files where the logging configuration is embedded within a larger
        // configuration file.
        //
        // loggerSection is the key of the key/value pair exposed by IConfigurationRoot which contains
        // the embedded logging configuration information. That embedded information must match the structure
        // defined by TConfig.
        //
        // channelTypes is a collection of Types implementing ILogChannel and decorated with a ChannelAttribute
        // defining the name/ID of the channel. Channel names/IDs should be unique. Type not fulfilling
        // the ILogChannel/ChannelAttribute constraints will be ignored. If duplicate Channel names/IDs are
        // specified only the last instance will be retained and used by the library.
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

            builder.RegisterLogger();

            return builder;
        }

        // utility method for creating and registering the underlying Serilog logger used by
        // J4JLogger.
        public static ContainerBuilder RegisterLogger(this ContainerBuilder builder)
        {
            builder.Register(c =>
               {
                   var loggerConfig = c.Resolve<IJ4JLoggerConfiguration>();
                   return loggerConfig.CreateLogger()!;
               })
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<J4JLogger>()
                .As<IJ4JLogger>();

            return builder;
        }

        // registers valid ILogChannel Types
        private static ContainerBuilder RegisterChannels(this ContainerBuilder builder, Dictionary<string, Type> channels)
        {
            foreach (var kvp in channels)
            {
                builder.RegisterType(kvp.Value)
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            return builder;
        }

        // validates Types as ILogChannel Types decorated with a ChannelAttribute. Types not meeting those constraints
        // are ignored. If duplicate channel names/IDs are specified (they should be unique) only the last
        // Type with the duplicate name/ID is retained.
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
