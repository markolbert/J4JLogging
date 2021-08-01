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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading;
using J4JSoftware.Logging;
using Serilog.Events;

namespace J4JLoggingTests
{
    public class DerivedData
    {
        public class Derived : LoggerInfo
        {
            public string SomeOtherProperty { get; set; } = string.Empty;
        }

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        static DerivedData()
        {
            var generator = new DataGenerator();

            TestData = generator.CreateData( 20 );

            WriteFiles();
        }

        public static List<object[]> TestData { get; }

        private static void WriteFiles()
        {
            foreach (var item in TestData)
            {
                var filePath = (string)item[0];

                if (File.Exists(filePath))
                    File.Delete(filePath);

                var loggerInfo = (LoggerInfo) item[ 1 ];

                var toSerialize = new Derived
                {
                    Global = loggerInfo.Global,
                    Channels = loggerInfo.Channels,
                    ChannelSpecific = loggerInfo.ChannelSpecific
                };

                File.WriteAllText(filePath, JsonSerializer.Serialize(toSerialize, _jsonOptions));
            }
        }
    }
}