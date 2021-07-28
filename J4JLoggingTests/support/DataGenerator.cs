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

            var data = CreateData(20);
        }

        public (List<LoggerInfo> loggerInfo, List<Object[]> filePaths) CreateData(int numFiles)
        {
            numFiles = numFiles <= 0 ? 5 : numFiles;

            var loggerInfo = new List<LoggerInfo>();
            var filePaths = new List<object[]>();

            for (var idx = 0; idx < numFiles; idx++)
            {
                var newLogInfo = new LoggerInfo
                {
                    Global = GetRandomChannelInfo(),
                    ChannelSpecific = GetRandomSpecificInfo()
                };

                loggerInfo.Add(newLogInfo);

                var filePath = Path.Combine(Environment.CurrentDirectory, $"config-file-{idx + 1}.json");
                filePaths.Add(new object[] { filePath });
            }

            return (loggerInfo, filePaths);
        }

        private ChannelInfo GetRandomChannelInfo()
        {
            return new ChannelInfo
            {
                IncludeSourcePath = _random.NextDouble() < 0.5,
                MinimumLevel = _levels[_random.Next(0, _levels.Length)],
                OutputTemplate = $"output template {_random.Next(0, 500)}",
                RequireNewLine = _random.NextDouble() < 0.5,
                SourceRootPath = $"root path {_random.Next(0, 500)}"
            };
        }

        private Dictionary<string, ChannelInfo> GetRandomSpecificInfo()
        {
            var retVal = new Dictionary<string, ChannelInfo>();

            var limit = _random.Next(0, _channels.Length);

            while (retVal.Count < limit)
            {
                var channelName = _channels[_random.Next(1, _channels.Length)];

                if (retVal.ContainsKey(channelName))
                    continue;

                retVal.Add(channelName, GetRandomChannelInfo());
            }

            return retVal;
        }
    }
}