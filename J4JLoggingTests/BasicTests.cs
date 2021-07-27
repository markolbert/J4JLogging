﻿#region license

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

using System;
using System.ComponentModel;
using System.IO;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace J4JLoggingTests
{
    public class BasicTests
    {
        private string _acctSID;
        private string _acctToken;
        private string _fromNumber;

        public BasicTests()
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddUserSecrets<BasicTests>()
                .Build();

            _acctSID = config.GetValue<string>( "twilio:AccountSID" );
            _acctToken = config.GetValue<string>( "twilio:AccountToken" );
            _fromNumber = config.GetValue<string>( "twilio:FromNumber" );
        }

        [ Fact ]
        public void Uncached()
        {
            var logger = new J4JLogger();

            logger.AddDebug();
            
            var twilio = logger.AddTwilio( _acctSID, _acctToken, _fromNumber );
            twilio.Parameters!.Recipients.Add( "+1 650 868 3367" );

            var lastEvent = (LastEventChannel) logger.AddChannel<LastEventChannel>();

            logger.SetLoggedType( GetType() );

            var template = "{0} (test message)";

            logger.Verbose<string>( template, "Verbose" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Verbose" ) );

            logger.Warning<string>( template, "Warning" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Warning" ) );

            logger.Information<string>( template, "Information" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Information" ) );

            logger.Debug<string>( template, "Debug" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Debug" ) );

            logger.Error<string>( template, "Error" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Error" ) );

            logger.Fatal<string>( template, "Fatal" );
            lastEvent.LastLogMessage.Should().Be( format_message( "Fatal" ) );

            logger.OutputNextEventToSms().Verbose<string>( "{0}", "Verbose" );

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" );
            }
        }

        [Fact]
        public void Cached()
        {
            var cached = new J4JCachedLogger();
            cached.SetLoggedType(GetType());

            var template = "{0} (test message)";

            cached.Verbose<string>(template, "Verbose");
            cached.Warning<string>(template, "Warning");
            cached.Information<string>(template, "Information");
            cached.Debug<string>(template, "Debug");
            cached.Error<string>(template, "Error");
            cached.Fatal<string>(template, "Fatal");
            cached.OutputNextEventToSms().Verbose<string>("{0} (test message)", "Verbose");

            var logger = new J4JLogger();

            logger.AddDebug();

            var twilio = logger.AddTwilio(_acctSID, _acctToken, _fromNumber);
            twilio.Parameters!.Recipients.Add("+1 650 868 3367");

            var lastEvent = (LastEventChannel)logger.AddChannel<LastEventChannel>();

            logger.SetLoggedType(GetType());

            foreach( var entry in cached.Entries )
            {
                if( entry.OutputToSms )
                    logger.OutputNextEventToSms();

                if( cached.LoggedType == null )
                    logger.ClearLoggedType();
                else logger.SetLoggedType( cached.LoggedType );

                logger.Write( entry.LogEventLevel, entry.Template, entry.PropertyValues, entry.MemberName,
                    entry.SourcePath, entry.SourceLine );

                lastEvent.LastLogMessage.Should().Be( format_message( entry.LogEventLevel.ToString() ) );
            }

            string format_message(string prop1)
            {
                return template.Replace("{0}", $"\"{prop1}\"");
            }
        }
    }
}