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
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    // defines the base configuration for a log channel
    public abstract class Channel<TParameters> : IChannel
        where TParameters : ChannelParameters
    {
        protected Channel(
            J4JLogger logger
        )
        {
            Logger = logger;
            Parameters = (TParameters) Activator.CreateInstance( typeof(TParameters), new object[] { logger } )!;
        }

        public J4JLogger Logger { get; }
        public LogEventLevel MinimumLevel => Parameters.MinimumLevel;
        public string EnrichedMessageTemplate => Parameters.EnrichedMessageTemplate;

        public TParameters Parameters { get; }

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}