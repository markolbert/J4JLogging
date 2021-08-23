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

using System.ComponentModel;
using FluentAssertions;
using J4JSoftware.Logging;
using Serilog.Events;
using Xunit;

namespace J4JLoggingTests
{
    public class NetEventTests : TestBase
    {
        private LogEventLevel _curLevel = LogEventLevel.Verbose;
        private string _curTemplate = string.Empty;

        [ Theory ]
        [ InlineData( LogEventLevel.Information ) ]
        [ InlineData( LogEventLevel.Error ) ]
        [ InlineData( LogEventLevel.Debug ) ]
        [ InlineData( LogEventLevel.Fatal ) ]
        [ InlineData( LogEventLevel.Warning ) ]
        [ InlineData( LogEventLevel.Verbose ) ]
        public void TestEvent( LogEventLevel level )
        {
            LogMessage( level );
        }

        private void LogMessage( LogEventLevel level )
        {
            _curLevel = level;

            var abbr = level switch
            {
                LogEventLevel.Debug => "DBG",
                LogEventLevel.Error => "ERR",
                LogEventLevel.Fatal => "FTL",
                LogEventLevel.Information => "INF",
                LogEventLevel.Verbose => "VRB",
                LogEventLevel.Warning => "WRN",
                _ => throw new InvalidEnumArgumentException( $"Unsupported {nameof(LogEventLevel)} '{level}'" )
            };

            _curTemplate = $"[{abbr}] This is a(n) \"{level}\" event\r\n";

            Logger.Write( level, "This is a(n) {0} event", level );
        }

        protected override void OnNetEvent( NetEventArgs e )
        {
            base.OnNetEvent( e );

            e.Level.Should().Be( _curLevel );
            e.LogMessage.Should().Be( _curTemplate );
        }
    }
}