#region license

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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class LoggerInfo
    {
        public LoggerInfo( 
            IConfiguration configRoot,
            string loggingSectionName = "Logging",
            string globalSettingsName = nameof(Global),
            string specificSettingsName = nameof(ChannelSpecific)
            )
        {
            var loggingSection = string.IsNullOrEmpty( loggingSectionName )
                ? configRoot
                : configRoot.GetSection( loggingSectionName );

            Global = loggingSection.GetSection( globalSettingsName )
                .Get<ChannelInfo>();

            var specificSection = loggingSection.GetSection( specificSettingsName )
                .GetChildren()
                .ToList();

            ChannelSpecific = loggingSection.GetSection( specificSettingsName )
                .Get<Dictionary<string, ChannelInfo?>>();
        }

        public LoggerInfo()
        {
        }

        public string LoggingSectionName { get; set; } = "Logging";
        public string GlobalSectionName { get; set; } = nameof(Global);
        public string ChannelsSectionName { get; set; } = nameof(Channels);
        public string SpecificSectionName { get; set; } = nameof(ChannelSpecific);

        public void Load( IConfiguration config )
        {
            var loggingSection = string.IsNullOrEmpty( LoggingSectionName )
                ? config
                : config.GetSection( LoggingSectionName );

            Global = loggingSection.GetSection(GlobalSectionName)
                .Get<ChannelInfo>();

            Channels = loggingSection.GetSection( ChannelsSectionName )
                .Get<List<string>>();

            var specificSections = loggingSection.GetSection(SpecificSectionName)
                .GetChildren()
                .ToList();

            foreach( var section in specificSections )
            {
                if( !Channels.Contains( section.Key ) )
                    Channels.Add( section.Key );
            }
        }

        public ChannelInfo? Global { get; set; }
        public List<string>? Channels { get; set; }
        public Dictionary<string, ChannelInfo?>? ChannelSpecific { get; set; }
    }
}