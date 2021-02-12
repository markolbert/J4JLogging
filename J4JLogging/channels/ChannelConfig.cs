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

using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public abstract class ChannelConfig : IChannelConfig
    {
        // the default Serilog message template to be used by the system
        public const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}";

        // the minimum Serilog level the channel will log
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Verbose;

        public string? OutputTemplate { get; set; } = DefaultOutputTemplate;

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the output template
        public EventElements EventElements { get; set; } = EventElements.All;

        public bool RequireNewline { get; set; } = true;

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );

        public virtual bool IsValid => true;

        // Gets the Serilog message template in use, augmented/enriched by optional fields
        // supported by the J4JLogger system (e.g., SourceContext, which represents the 
        // source code file's path).
        public virtual string EnrichedMessageTemplate
        {
            get
            {
                var sb = new StringBuilder( OutputTemplate );

                foreach( var element in EnumExtensions.GetUniqueFlags<EventElements>() )
                {
                    var inclElement = ( EventElements & element ) == element;

                    switch( element )
                    {
                        case EventElements.Type:
                            if( inclElement )
                                sb.Append( " {SourceContext}{MemberName}" );

                            break;

                        case EventElements.SourceCode:
                            if( inclElement )
                                sb.Append( " {SourceCodeInformation}" );

                            break;
                    }
                }

                if( RequireNewline )
                    sb.Append( "{NewLine}" );

                return sb.ToString();
            }
        }
    }
}