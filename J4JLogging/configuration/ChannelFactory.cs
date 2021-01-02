using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class ChannelFactory : IChannelFactory
    {
        private readonly Dictionary<string, Type> _channels =
            new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public ChannelFactory( 
            IConfigurationRoot configRoot, 
            string? rootKey = null,
            bool inclLastEvent = false 
            )
        {
            ConfigurationRoot = configRoot;
            RootKey = rootKey;
            LastEvent = inclLastEvent ? new LastEventConfig() : null;
        }

        public IConfigurationRoot ConfigurationRoot { get; }
        public string? RootKey { get; }
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

            retVal = string.IsNullOrEmpty( RootKey )
                ? ConfigurationRoot.Get<TJ4JLogger>()
                : ConfigurationRoot.GetSection( RootKey ).Get<TJ4JLogger>();

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
                        ? ConfigurationRoot.GetSection( elements[ idx ] ) 
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
