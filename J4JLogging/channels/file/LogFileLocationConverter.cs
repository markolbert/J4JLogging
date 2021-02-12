#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLogging' is free software: you can redistribute it
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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace J4JSoftware.Logging
{
    // converts between string values and LogFileLocation enum values. Any text other than 
    // "appdata" (case insensitive) translates to LogFileLocation.ExeFolder.
    public class LogFileLocationConverter : JsonConverter<LogFileLocation>
    {
        public override LogFileLocation Read( ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options )
        {
            return reader.GetString()!.ToLowerInvariant() switch
            {
                "appdata" => LogFileLocation.AppData,
                _ => LogFileLocation.ExeFolder
            };
        }

        public override void Write( Utf8JsonWriter writer, LogFileLocation value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}