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

using System.Collections.Generic;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace J4JLoggingTests
{
    public class TestBase
    {
        protected TestBase()
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddUserSecrets<LoggingTests>()
                .Build();

            var twilioConfig = new TwilioConfiguration
            {
                AccountSID = config.GetValue<string>( "twilio:AccountSID" ),
                AccountToken = config.GetValue<string>( "twilio:AccountToken" ),
                FromNumber = config.GetValue<string>( "twilio:FromNumber" ),
                Recipients = new List<string> { "+1 650 868 3367" }
            };

            Logger = new J4JLogger();

            Logger.ReportLoggedType()
                .ReportCallingMember()
                .ReportLineNumber()
                .ReportSourceCodeFile()
                .IncludeSendToTwilio( twilioConfig );

            Logger.LoggerConfiguration
                .WriteTo.Debug( outputTemplate: Logger.GetOutputTemplate(true) )
                .WriteTo.LastEvent( out var temp )
                .WriteTo.NetEvent( out var temp2 )
                .MinimumLevel.Verbose();

            LastEvent = temp!;

            NetEvent = temp2;
            NetEvent.LogEvent += LogEvent;

            Logger.Create();
            Logger.Built.Should().BeTrue();
        }

        private void LogEvent( object? sender, NetEventArgs e ) => OnNetEvent( e );

        protected virtual void OnNetEvent( NetEventArgs e )
        {
        }

        protected J4JLogger Logger { get; }
        protected LastEventSink LastEvent { get; }
        protected NetEventSink NetEvent { get; }

    }
}