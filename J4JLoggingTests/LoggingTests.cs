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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FluentAssertions;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Xunit;

namespace J4JLoggingTests
{
    public class LoggingTests
    {
        private readonly TwilioConfiguration _twilioConfig;

        public LoggingTests()
        {
            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder
                .AddUserSecrets<LoggingTests>()
                .Build();

            _twilioConfig = new TwilioConfiguration
            {
                IncludeSourcePath = true,
                SourceRootPath = null,
                OutputTemplate = J4JBaseLogger.DefaultOutputTemplate,
                RequireNewLine = true,
                MinimumLevel = LogEventLevel.Verbose,
                AccountSID = config.GetValue<string>( "twilio:AccountSID" ),
                AccountToken = config.GetValue<string>( "twilio:AccountToken" ),
                FromNumber = config.GetValue<string>( "twilio:FromNumber" ),
                Recipients = new List<string> { "+1 650 868 3367" }
            };
        }

        [ Fact ]
        public void Uncached()
        {
            var logger = new J4JLogger();
            logger.SetLoggedType(GetType());

            logger.AddDebug();
            logger.AddTwilio( _twilioConfig );

            var lastEvent = logger.AddLastEvent();

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
            cached.OutputNextEventToSms().Verbose<string>( "{0} (test message)", "Verbose" );

            var logger = new J4JLogger();

            logger.AddDebug();
            logger.AddTwilio( _twilioConfig );

            var lastEvent = logger.AddLastEvent();

            logger.SetLoggedType( GetType() );

            foreach( var entry in cached.Entries )
            {
                if( entry.OutputToSms )
                    logger.OutputNextEventToSms();

                if( cached.LoggedType == null )
                    logger.ClearLoggedType();
                else logger.SetLoggedType( cached.LoggedType );

                logger.Write( entry.LogEventLevel, entry.MessageTemplate, entry.PropertyValues, entry.MemberName,
                    entry.SourcePath, entry.SourceLine );

                lastEvent.LastLogMessage.Should().Be( format_message( entry.LogEventLevel.ToString() ) );
            }

            string format_message( string prop1 )
            {
                return template.Replace( "{0}", $"\"{prop1}\"" );
            }
        }
    }
}