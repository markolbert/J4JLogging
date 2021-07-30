using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class J4JLoggerConfigurator
    {
        private record MapEntry( string LibraryName, string LocalName );

        private readonly Dictionary<string, ChannelIDAttribute> _channels = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<MapEntry> _nameMap = new();

        public J4JLoggerConfigurator(
            IEnumerable<IChannel> channels
        )
        {
            foreach( var channel in channels )
            {
                var idAttr = channel.GetType()
                    .GetCustomAttributes( typeof(ChannelIDAttribute), false )
                    .Cast<ChannelIDAttribute>()
                    .ToList();

                if( idAttr.Count != 1
                    || _nameMap.Any( x =>
                        x.LibraryName.Equals( idAttr[ 0 ].Name, StringComparison.OrdinalIgnoreCase ) ) )
                    continue;

                _nameMap.Add( new MapEntry( idAttr[ 0 ].Name, idAttr[ 0 ].Name ) );

                _channels.Add( idAttr[ 0 ].Name, idAttr[ 0 ] );
            }
        }

        public string LoggingSectionName { get; set; } = "Logging";
        public string GlobalSectionName { get; set; } = "Global";
        public string ChannelSpecificSectionName { get; set; } = "ChannelSpecific";

        public bool MapChannelName( string libraryName, string localName )
        {
            var mapIndex = _nameMap.FindIndex( x =>
                x.LibraryName.Equals( libraryName, StringComparison.OrdinalIgnoreCase ) );

            if( mapIndex < 0 )
                return false;

            _nameMap[ mapIndex ] = new MapEntry( libraryName, localName );

            return true;
        }

        public void ConfigureLogger( J4JLogger logger, IConfiguration configuration )
        {
            var loggerInfo = new LoggerInfo( configuration, 
                LoggingSectionName, 
                GlobalSectionName,
                ChannelSpecificSectionName );

            logger.ApplySettings( loggerInfo );

            if( loggerInfo.ChannelSpecific == null )
                return;

            var genericType = typeof(Channel<>);

            foreach( var kvp in loggerInfo.ChannelSpecific )
            {
                var mappedName = _nameMap.FirstOrDefault( x =>
                    x.LocalName.Equals( kvp.Key, StringComparison.OrdinalIgnoreCase ) );

                if( mappedName == null || !_channels.ContainsKey( mappedName.LibraryName ) )
                    continue;

                var channelID = _channels[ mappedName.LibraryName ];

                var channel = Activator.CreateInstance( channelID.ChannelType, new object[] { logger } ) as IChannel;
                if( channel == null )
                    continue;

                logger.Channels.Add(channel);
            }
        }
    }
}
