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
    // defines the functionality of a type that can be used to configure the J4JLogger
    // system
    public interface IJ4JLoggerConfiguration //: IEnumerable<IChannelConfig>
    {
        // The root path of source code files. Used to eliminate redundant path information in the
        // logging output (i.e., by supressing common path elements)
        string? SourceRootPath { get; }

        // flag indicating whether or not multi line events are supported
        bool MultiLineEvents { get; }

        // flag indicating which event elements (e.g., type information, source code information)
        // will be added to the logging output
        EventElements EventElements { get; set; }

        List<IChannelConfig> Channels { get; }
    }

    //public interface IJ4JLoggerConfiguration<TChannels> : IJ4JLoggerConfiguration
    //    where TChannels : ILogChannels, new()
    //{
    //    TChannels Channels { get; set; }
    //}
}