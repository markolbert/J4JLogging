using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class LoggerConfigurator : ILoggerConfigurator
    {
        private readonly Func<J4JLogger, Type, IChannel?> _channelFactory;
        private readonly Dictionary<string, Type> _nameTypeMap = new(StringComparer.OrdinalIgnoreCase);

        public LoggerConfigurator(
            Func<J4JLogger, Type, IChannel?> channelFactory
        )
        {
            _channelFactory = channelFactory;

            ScanAssemblyForChannels();
        }

        protected void ScanAssemblyForChannels()
        {
            foreach( var type in this.GetType().Assembly.DefinedTypes.Where(t => !t.IsAbstract
                && typeof(IChannel).IsAssignableFrom(t)
                && t.GetConstructors().Any(c => c.GetParameters().Length == 0 )))
            {
                var channelAttr = type.GetCustomAttribute<ChannelIDAttribute>(false);
                if( channelAttr == null )
                    continue;

                if( _nameTypeMap.ContainsKey( channelAttr.Name ) )
                    continue;

                _nameTypeMap.Add( channelAttr.Name, channelAttr.ChannelType );
            }
        }

        public J4JLogger Configure( J4JLogger logger, object? loggerInfo, params string[] channels ) =>
            Configure( logger, loggerInfo is LoggerInfo castInfo ? castInfo : null, channels );

        public virtual J4JLogger Configure( J4JLogger logger, LoggerInfo? loggerInfo, params string[] channels)
        {
            if( loggerInfo == null )
                return logger;

            logger.ConfigureLogger(loggerInfo.Global);

            foreach (var channelName in loggerInfo.AllChannels(channels))
            {
                if( !_nameTypeMap.ContainsKey( channelName ) )
                    continue;

                var channelType = _nameTypeMap[ channelName ];

                var newChannel = _channelFactory(logger, channelType);

                if (newChannel == null)
                    continue;

                var configValues = loggerInfo.ChannelSpecific!.ContainsKey(channelName)
                    ? loggerInfo.ChannelSpecific[channelName]
                    : null;

                switch (newChannel)
                {
                    case FileChannel fileChannel:
                        fileChannel.ConfigureFileChannel((FileConfiguration?)configValues);
                        break;

                    case ConsoleChannel consoleChannel:
                        consoleChannel.ConfigureChannel(configValues);
                        break;

                    case DebugChannel debugChannel:
                        debugChannel.ConfigureChannel(configValues);
                        break;

                    case NetEventChannel netEventChannel:
                        netEventChannel.ConfigureChannel(configValues);
                        break;

                    case LastEventChannel lastEventChannel:
                        lastEventChannel.ConfigureChannel(configValues);
                        break;
                }

                logger.Channels.Add(newChannel);
            }

            return logger;
        }
    }
}
