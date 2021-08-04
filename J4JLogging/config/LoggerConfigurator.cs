using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class LoggerConfigurator : ILoggerConfigurator
    {
        private readonly IChannelFactory _channelFactory;

        public LoggerConfigurator(
            IChannelFactory channelFactory
        )
        {
            _channelFactory = channelFactory;
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
                var newChannel = _channelFactory.GetLoggerChannel(logger, channelName);

                if (newChannel == null)
                    continue;

                var configValues = loggerInfo.ChannelSpecific!.ContainsKey(channelName)
                    ? loggerInfo.ChannelSpecific[channelName]
                    : null;

                if( ConfigureChannel( newChannel, configValues, out var configuredChannel ) )
                    logger.Channels.Add( configuredChannel! );
            }

            return logger;
        }

        protected virtual bool ConfigureChannel( IChannel channel, ChannelConfiguration? config, out IChannel? result )
        {
            result = channel switch
            {
                FileChannel fileChannel => fileChannel.ConfigureFileChannel( (FileConfiguration?) config ),
                ConsoleChannel consoleChannel => consoleChannel.ConfigureChannel( config ),
                DebugChannel debugChannel => debugChannel.ConfigureChannel( config ),
                NetEventChannel netEventChannel => netEventChannel.ConfigureChannel( config ),
                LastEventChannel lastEventChannel => lastEventChannel.ConfigureChannel( config ),
                _ => null
            };

            return result != null;
        }
    }
}
