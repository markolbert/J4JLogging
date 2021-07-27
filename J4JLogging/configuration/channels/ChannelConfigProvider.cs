﻿#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
// and/or modify it under the terms of the GNU General Public License as
// published by the Free Software Foundation, either version 3 of the License,
// or (at your option) any later version.
// 
// This library or program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with
// this library or program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class ChannelConfigProvider : IChannelConfigProvider
    {
        private readonly Dictionary<string, Type> _configurableChannels = new( StringComparer.OrdinalIgnoreCase );
        private readonly List<IChannelParameters> _configuredChannels = new();
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

                if( curSection.Get( kvp.Value ) is IChannelParameters curConfig )
                    retVal.Channels.Add( curConfig );
            }

            retVal.Channels.AddRange( _configuredChannels );

            if( LastEvent != null )
                retVal.Channels.Add( LastEvent );

            return retVal;
        }

        public ChannelConfigProvider AddChannel<TChannel>( string configPath )
            where TChannel : IChannelParameters, new()
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
                || !typeof(IChannelParameters).IsAssignableFrom( channelType ) )
                return this;

            if( _configurableChannels.ContainsKey( configPath ) )
                _configurableChannels[ configPath ] = channelType;
            else _configurableChannels.Add( configPath, channelType );

            return this;
        }

        public ChannelConfigProvider AddChannel( IChannelParameters channelConfig )
        {
            _configuredChannels.Add( channelConfig );

            return this;
        }
    }
}