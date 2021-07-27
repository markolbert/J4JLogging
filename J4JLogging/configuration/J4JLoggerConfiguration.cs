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

using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    // a type that can be used to configure the J4JLogger system
    public class J4JLoggerConfiguration : IJ4JLoggerConfiguration
    {
        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        public string? SourceRootPath { get; set; }

        // flag indicating whether or not multi line events are supported
        public bool MultiLineEvents { get; set; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        public EventElements EventElements { get; set; } = EventElements.All;

        // configuration information for the log channels
        public List<IChannelParameters> Channels { get; } = new();
    }
}