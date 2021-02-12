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
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;

namespace J4JSoftware.Logging
{
    // defines the configuration for a channel that retains the text of the last
    // even logged
    public class NetEventConfig : ChannelConfig
    {
        public const string DefaultNetEventConfigOutputTemplate = "[{Level:u3}] {Message}";

        public NetEventConfig()
        {
            OutputTemplate = DefaultNetEventConfigOutputTemplate;
            EventElements = EventElements.None;
            RequireNewline = false;
        }

        public event EventHandler<NetEventArgs>? LogEvent;

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            return sinkConfig.NetEvent( new MessageTemplateTextFormatter( EnrichedMessageTemplate ), this );
        }

        internal void OnLogEvent( NetEventArgs args )
        {
            LogEvent?.Invoke( this, args );
        }
    }
}