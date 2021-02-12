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
using Serilog;

namespace J4JSoftware.Logging
{
    // converts between string values and RollingInterval values. Any text other than day, hour, minute,
    // month or year (case insensitive) maps to RollingInterval.Infinite.
    public class RollingIntervalConverter : JsonConverter<RollingInterval>
    {
        public override RollingInterval Read( ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options )
        {
            return reader.GetString()!.ToLowerInvariant() switch
            {
                "day" => RollingInterval.Day,
                "hour" => RollingInterval.Hour,
                "minute" => RollingInterval.Minute,
                "month" => RollingInterval.Month,
                "year" => RollingInterval.Year,
                _ => RollingInterval.Infinite
            };
        }

        public override void Write( Utf8JsonWriter writer, RollingInterval value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}