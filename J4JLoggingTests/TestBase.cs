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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace J4JLoggingTests
{
    public class TestBase
    {
        private static string ConvertCallingContextToText(
            Type? loggedType,
            string callerName,
            int lineNum,
            string srcFilePath)
        {
            return CallingContextEnricher.DefaultConvertToText(loggedType,
                callerName,
                lineNum,
                CallingContextEnricher.RemoveProjectPath(srcFilePath, GetProjectPath()));
        }

        private static string GetProjectPath([CallerFilePath] string filePath = "")
        {
            var dirInfo = new DirectoryInfo(Path.GetDirectoryName(filePath)!);

            while (dirInfo.Parent != null)
            {
                if (dirInfo.EnumerateFiles("*.csproj").Any())
                    break;

                dirInfo = dirInfo.Parent;
            }

            return dirInfo.FullName;
        }

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

            var loggerConfig = new J4JLoggerConfiguration()
                {
                    CallingContextToText = ConvertCallingContextToText
                }
                .AddEnricher<CallingContextEnricher>()
                .IncludeSendToTwilio( twilioConfig );

            loggerConfig.SerilogConfiguration
                .WriteTo.Debug( outputTemplate: J4JLoggerConfiguration.GetOutputTemplate( true ) )
                .WriteTo.LastEvent( out var temp )
                .WriteTo.NetEvent( out var temp2 );

            LastEvent = temp!;

            NetEvent = temp2;
            NetEvent.LogEvent += LogEvent;

            Logger = loggerConfig.CreateLogger();
            Logger.SetLoggedType( GetType() );
        }

        private void LogEvent( object? sender, NetEventArgs e ) => OnNetEvent( e );

        protected virtual void OnNetEvent( NetEventArgs e )
        {
        }

        protected IJ4JLogger Logger { get; }
        protected LastEventSink LastEvent { get; }
        protected NetEventSink NetEvent { get; }

    }
}