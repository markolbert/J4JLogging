#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLoggingTests' is free software: you can redistribute it
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

using FluentAssertions;
using J4JSoftware.Logging;
using Xunit;

namespace J4JLoggingTests
{
    public class LoggingTests : TestBase
    {
        [ Fact ]
        public void Uncached()
        {
            var template = "{0} (test message)";

            Logger.Verbose<string>( template, "Verbose" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Verbose" ) );

            Logger.Warning<string>( template, "Warning" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Warning" ) );

            Logger.Information<string>( template, "Information" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Information" ) );

            Logger.Debug<string>( template, "Debug" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Debug" ) );

            Logger.Error<string>( template, "Error" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Error" ) );

            Logger.Fatal<string>( template, "Fatal" );
            LastEvent.LastLogMessage.Should().Be( format_message( "Fatal" ) );

            Logger.SmsHandling = SmsHandling.SendNextMessage;
            Logger.Verbose<string>( "{0}", "Verbose" );

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" );
            }
        }

        [ Fact ]
        public void Cached()
        {
            var cached = new J4JCachedLogger();
            cached.SetLoggedType( GetType() );

            var template = "{0} (test message)";

            cached.Verbose<string>( template, "Verbose" );
            cached.Warning<string>( template, "Warning" );
            cached.Information<string>( template, "Information" );
            cached.Debug<string>( template, "Debug" );
            cached.Error<string>( template, "Error" );
            cached.Fatal<string>( template, "Fatal" );

            cached.SmsHandling = SmsHandling.SendNextMessage;
            cached.Verbose<string>( "{0} (test message)", "Verbose" );

            foreach( var entry in cached.Entries )
            {
                Logger.SmsHandling = entry.SmsHandling;

                if( cached.LoggedType != null )
                    Logger.SetLoggedType( cached.LoggedType );

                Logger.Write( entry.LogEventLevel,
                             entry.MessageTemplate,
                             entry.PropertyValues,
                             entry.MemberName,
                             entry.SourcePath,
                             entry.SourceLine );

                LastEvent.LastLogMessage.Should().Be( format_message( entry.LogEventLevel.ToString() ) );
            }

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" );
            }
        }
    }
}
