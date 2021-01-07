using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class ChannelFactory : IChannelFactory
    {
        private readonly IConfiguration _config;
        private readonly string? _loggingSectionKey;
        private readonly ChannelInformation _channelInfo;

        public ChannelFactory( 
            IConfiguration config, 
            ChannelInformation channelInfo,
            string? loggingSectionKey = null,
            bool inclLastEvent = false 
            )
        {
            _config = config;
            _channelInfo = channelInfo;
            _loggingSectionKey = loggingSectionKey;
            LastEvent = inclLastEvent ? new LastEventConfig() : null;
        }

        public LastEventConfig? LastEvent { get; }

        //public bool AddChannel<TChannel>(string configPath)
        //    where TChannel : IChannelConfig, new()
        //{
        //    if (string.IsNullOrEmpty(configPath))
        //        return false;

        //    if (_channelInfo.ContainsKey(configPath))
        //        _channelInfo[configPath] = typeof(TChannel);
        //    else _channelInfo.Add(configPath, typeof(TChannel));

        //    return true;
        //}

        //public bool AddChannel(Type channelType, string configPath)
        //{
        //    if (string.IsNullOrEmpty(configPath) 
        //        || !typeof(IChannelConfig).IsAssignableFrom(channelType))
        //        return false;

        //    if (_channelInfo.ContainsKey(configPath))
        //        _channelInfo[configPath] = channelType;
        //    else _channelInfo.Add(configPath, channelType);

        //    return true;
        //}

        public IJ4JLoggerConfiguration? GetLoggerConfiguration<TJ4JLogger>()
            where TJ4JLogger: IJ4JLoggerConfiguration, new()
        {
            IJ4JLoggerConfiguration? retVal = null;

            retVal = string.IsNullOrEmpty( _loggingSectionKey )
                ? _config.Get<TJ4JLogger>()
                : _config.GetSection( _loggingSectionKey ).Get<TJ4JLogger>();

            retVal.SetChannels( this );

            return retVal;
        }

        public IEnumerator<IChannelConfig> GetEnumerator()
        {
            foreach (var kvp in _channelInfo)
            {
                var elements = kvp.Key.Split( ':', StringSplitOptions.RemoveEmptyEntries );
                if( elements.Length == 0 )
                    continue;

                var idx = 0;
                IConfigurationSection? curSection = null;

                do
                {
                    curSection = curSection == null 
                        ? _config.GetSection( elements[ idx ] ) 
                        : curSection.GetSection( elements[ idx ] );

                    idx++;
                } while ( idx < elements.Length );

                if( curSection.Get( kvp.Value ) is IChannelConfig curConfig )
                    yield return curConfig;
            }

            if (LastEvent != null)
                yield return LastEvent;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
