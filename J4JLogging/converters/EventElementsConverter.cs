﻿#region license

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
    // converts between string values and EventElements enum values. Any recognized text
    // results results in an EventElements.Basic value.
    public class EventElementsConverter : JsonConverter<EventElements>
    {
        public override EventElements Read( ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options )
        {
            // we return Basic if something goes wrong
            try
            {
                return Enum.Parse<EventElements>( reader.GetString()!, true );
            }
            catch
            {
                return EventElements.All;
            }
        }

        public override void Write( Utf8JsonWriter writer, EventElements value, JsonSerializerOptions options )
        {
            writer.WriteStringValue( value.ToString() );
        }
    }
}