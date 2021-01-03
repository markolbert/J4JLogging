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
        private readonly Dictionary<string, Type> _channels =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public ChannelFactory( 
            IConfiguration config, 
            string? loggingSectionKey = null,
            bool inclLastEvent = false 
            )
        {
            _config = config;
            _loggingSectionKey = loggingSectionKey;
            LastEvent = inclLastEvent ? new LastEventConfig() : null;
        }

        public LastEventConfig? LastEvent { get; }

        public bool AddChannel<TChannel>(string configPath)
            where TChannel : IChannelConfig, new()
        {
            if (string.IsNullOrEmpty(configPath))
                return false;

            if (_channels.ContainsKey(configPath))
                _channels[configPath] = typeof(TChannel);
            else _channels.Add(configPath, typeof(TChannel));

            return true;
        }

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
            foreach (var kvp in _channels)
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
