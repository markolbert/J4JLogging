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
        private TParameters? _channelParams;

        protected Channel(
            J4JLogger logger
        )
        {
            Logger = logger;
        }

        public J4JLogger Logger { get; }

        public bool LocallyDefined => _channelParams != null;

        public LogEventLevel MinimumLevel => _channelParams?.MinimumLevel ?? Logger.Parameters.MinimumLevel;

        public string EnrichedMessageTemplate =>
            _channelParams?.EnrichedMessageTemplate ?? Logger.Parameters.EnrichedMessageTemplate;

        public TParameters? Parameters
        {
            get => _channelParams;

            set
            {
                _channelParams = value;
                Logger.ResetBaseLogger();
            }
        }

        public void ResetToGlobal() => _channelParams = null;

        public abstract LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig );
    }
}