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
using System.Text.Json;
using System.Threading;
using J4JSoftware.Logging;
using Serilog;
using Serilog.Events;

namespace J4JLoggingTests
{
    public class DataGenerator
    {
        private readonly Random _random;
        private readonly LogEventLevel[] _levels;
        private readonly string[] _channels;

        public DataGenerator()
        {
            Thread.Sleep(1);
            _random = new Random();

            _levels = Enum.GetValues<LogEventLevel>();
            _channels = new[] { "Console", "Debug", "File", "Twilio", "LastEvent" };
        }

        public J4JLogger Logger { get; } = new();

        public List<object[]> CreateData( int numFiles )
        {
            numFiles = numFiles <= 0 ? 5 : numFiles;

            var retVal = new List<object[]>();

            for( var idx = 0; idx < numFiles; idx++ )
            {
                retVal.Add( new object[]
                {
                    Path.Combine( Environment.CurrentDirectory, $"config-file-{idx + 1}.json" ),

                    new LoggerInfo
                    {
                        Global = GetRandomChannelInfo( null ),
                        ChannelSpecific = GetRandomSpecificInfo()
                    }
                } );
            }

            return retVal;
        }

        private ChannelConfiguration GetRandomChannelInfo( string? channelName )
        {
            var retVal = channelName?.ToLower() switch
            {
                "file" => new FileConfiguration(),
                "twilio" => new TwilioConfiguration(),
                _ => new ChannelConfiguration()
            };

            if( _random.NextDouble() < 0.5 )
                retVal.IncludeSourcePath = true;

            retVal.MinimumLevel = _levels[ _random.Next( 0, _levels.Length ) ];
            retVal.OutputTemplate = $"output template {_random.Next( 0, 500 )}";
            retVal.RequireNewLine = _random.NextDouble() < 0.5;
            retVal.SourceRootPath = $"root path {_random.Next( 0, 500 )}";

            switch( retVal )
            {
                case FileConfiguration fileParameters:
                    fileParameters.FileName = $"random-log-file-{_random.Next( 1, 100 )}.txt";
                    fileParameters.Folder = $"c:/log-folder-{_random.Next( 1, 100 )}";

                    fileParameters.RollingInterval = _random.NextDouble() > 0.5
                        ? RollingInterval.Day
                        : RollingInterval.Hour;
                    break;

                case TwilioConfiguration twilioParameters:
                    twilioParameters.AccountToken = $"random-account-token-{_random.Next( 0, 500 )}";
                    twilioParameters.AccountSID = $"random-account-SID-{_random.Next( 0, 500 )}";
                    twilioParameters.FromNumber = $"{_random.Next( 100000000, 999999999 ) * 10}";
                    break;
            }

            return retVal;
        }

        private Dictionary<string, ChannelConfiguration> GetRandomSpecificInfo()
        {
            var retVal = new Dictionary<string, ChannelConfiguration>();

            var limit = _random.Next( 0, _channels.Length );

            while( retVal.Count < limit )
            {
                var channelName = _channels[ _random.Next( 1, _channels.Length ) ];

                if( retVal.ContainsKey( channelName ) )
                    continue;

                if( _random.NextDouble() < 0.5 )
                    retVal.Add( channelName, GetRandomChannelInfo( channelName ) );
            }

            return retVal;
        }
    }
}