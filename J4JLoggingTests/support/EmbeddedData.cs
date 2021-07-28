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
    public class EmbeddedData
    {
        public class Embedded
        {
            public LoggerInfo? LoggerInfo { get; set; }
        }

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        static EmbeddedData()
        {
            var generator = new DataGenerator();
            var data = generator.CreateData( 20 );

            LoggerInfoItems = data.loggerInfo;
            FilePaths = data.filePaths;

            WriteFiles( data.loggerInfo, data.filePaths );
        }

        public static List<LoggerInfo> LoggerInfoItems { get; }
        public static List<object[]> FilePaths { get; }

        public static LoggerInfo? GetLoggerInfo( string filePath )
        {
            var idx = FilePaths.FindIndex( x =>
                filePath.Equals( (string) x[ 0 ], StringComparison.OrdinalIgnoreCase ) );

            if( idx < 0 )
                return null;

            return LoggerInfoItems[ idx ];
        }

        private static void WriteFiles( List<LoggerInfo> items, List<object[]> filePaths )
        {
            if( items.Count != filePaths.Count )
                throw new ArgumentException( $"Differing number of LoggerInfo items and file paths" );

            for( var idx = 0; idx < items.Count; idx++ )
            {
                var filePath = (string) filePaths[ idx ][ 0 ];

                if( File.Exists( filePath ) )
                    File.Delete( filePath );

                var toSerialize = new Embedded() { LoggerInfo = items[ idx ] };

                File.WriteAllText( filePath, JsonSerializer.Serialize( toSerialize, _jsonOptions ) );
            }
        }
    }
}