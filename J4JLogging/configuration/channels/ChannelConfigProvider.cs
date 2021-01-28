using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class ChannelConfigProvider : IChannelConfigProvider
    {
        private readonly Dictionary<string, Type> _configurableChannels = new( StringComparer.OrdinalIgnoreCase );
        private readonly List<IChannelConfig> _configuredChannels = new();
        private readonly string? _loggerSectionKey;

        public ChannelConfigProvider(
            string? loggerSectionKey = null,
            bool inclLastEvent = false
        )
        {
            _loggerSectionKey = loggerSectionKey;

            if( inclLastEvent )
                LastEvent = new LastEventConfig();
        }

        public IConfiguration? Source { get; set; }
        public LastEventConfig? LastEvent { get; }

        public ChannelConfigProvider AddChannel<TChannel>( string configPath )
            where TChannel : IChannelConfig, new()
        {
            if( string.IsNullOrEmpty( configPath ) )
                return this;

            if( _configurableChannels.ContainsKey( configPath ) )
                _configurableChannels[ configPath ] = typeof(TChannel);
            else _configurableChannels.Add( configPath, typeof(TChannel) );

            return this;
        }

        public ChannelConfigProvider AddChannel( Type channelType, string configPath )
        {
            if( string.IsNullOrEmpty( configPath )
                || !typeof(IChannelConfig).IsAssignableFrom( channelType ) )
                return this;

            if( _configurableChannels.ContainsKey( configPath ) )
                _configurableChannels[ configPath ] = channelType;
            else _configurableChannels.Add( configPath, channelType );

            return this;
        }

        public ChannelConfigProvider AddChannel( IChannelConfig channelConfig )
        {
            _configuredChannels.Add( channelConfig );

            return this;
        }

        public TJ4JLogger? GetConfiguration<TJ4JLogger>()
            where TJ4JLogger : class, IJ4JLoggerConfiguration, new()
        {
            if( Source == null )
                throw new NullReferenceException(
                    $"{nameof(Source)} is undefined. It must be assigned an {nameof(IConfiguration)} object" );

            var retVal = string.IsNullOrEmpty( _loggerSectionKey )
                ? Source.Get<TJ4JLogger>()
                : Source.GetSection( _loggerSectionKey ).Get<TJ4JLogger>();

            if( retVal == null )
                return retVal;

            foreach( var kvp in _configurableChannels )
            {
                var elements = kvp.Key.Split( ':', StringSplitOptions.RemoveEmptyEntries )
                    .ToList();

                if( !string.IsNullOrEmpty( _loggerSectionKey ) )
                    elements.Insert( 0, _loggerSectionKey );

                if( elements.Count == 0 )
                    continue;

                var idx = 0;
                IConfigurationSection? curSection = null;

                do
                {
                    curSection = curSection == null
                        ? Source.GetSection( elements[ idx ] )
                        : curSection.GetSection( elements[ idx ] );

                    idx++;
                } while( idx < elements.Count );

                if( curSection.Get( kvp.Value ) is IChannelConfig curConfig )
                    retVal.Channels.Add( curConfig );
            }

            retVal.Channels.AddRange( _configuredChannels );

            if( LastEvent != null )
                retVal.Channels.Add( LastEvent );

            return retVal;
        }
    }
}