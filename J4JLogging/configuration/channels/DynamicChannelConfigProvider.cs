using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class DynamicChannelConfigProvider : ChannelConfigProviderBase
    {
        private readonly Dictionary<string, Type> _channels = new( StringComparer.OrdinalIgnoreCase );

        public DynamicChannelConfigProvider(
            string? loggerSectionKey = null,
            bool includeLastEvent = false,
            IJ4JLogger? logger = null )
            : base( includeLastEvent, logger )
        {
            LoggerSectionKey = loggerSectionKey;
        }

        public string? LoggerSectionKey { get; }
        public IConfiguration? Source { get; set; }

        public DynamicChannelConfigProvider AddChannel<TChannel>( string configPath )
            where TChannel : IChannelConfig, new()
        {
            if( string.IsNullOrEmpty( configPath ) )
                return this;

            if( _channels.ContainsKey( configPath ) )
                _channels[ configPath ] = typeof(TChannel);
            else _channels.Add( configPath, typeof(TChannel) );

            return this;
        }

        public DynamicChannelConfigProvider AddChannel( Type channelType, string configPath )
        {
            if( string.IsNullOrEmpty( configPath )
                || !typeof(IChannelConfig).IsAssignableFrom( channelType ) )
                return this;

            if( _channels.ContainsKey( configPath ) )
                _channels[ configPath ] = channelType;
            else _channels.Add( configPath, channelType );

            return this;
        }

        public override void AddChannelsToLoggerConfiguration<TJ4JLogger>( TJ4JLogger? loggerConfig = null )
            where TJ4JLogger: class
        {
            if( Source == null )
                return;

            loggerConfig ??= string.IsNullOrEmpty( LoggerSectionKey )
                ? Source.Get<TJ4JLogger>()
                : Source.GetSection( LoggerSectionKey ).Get<TJ4JLogger>();

            loggerConfig.Channels.AddRange( EnumerateChannels() );
        }

        protected override IEnumerable<IChannelConfig> EnumerateChannels()
        {
            if( Source == null )
                yield break;

            foreach( var kvp in _channels )
            {
                var elements = kvp.Key.Split( ':', StringSplitOptions.RemoveEmptyEntries )
                    .ToList();

                if( !string.IsNullOrEmpty( LoggerSectionKey ) )
                    elements.Insert( 0, LoggerSectionKey );

                if( elements.Count == 0 )
                    continue;

                var idx = 0;
                IConfigurationSection? curSection = null;

                do
                {
                    curSection = curSection == null
                        ? Source!.GetSection( elements[ idx ] )
                        : curSection.GetSection( elements[ idx ] );

                    idx++;
                } while( idx < elements.Count );

                if( curSection.Get( kvp.Value ) is IChannelConfig curConfig )
                    yield return curConfig;
            }

            if( LastEvent != null )
                yield return LastEvent;
        }
    }
}