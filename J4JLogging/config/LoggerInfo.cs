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
using System.Collections.Generic;
using System.Linq;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class LoggerInfo
    {
        public ChannelConfiguration Global { get; set; } = new();
        public List<string> Channels { get; set; } = new();

        public Dictionary<string, ChannelConfiguration> ChannelSpecific { get; set; } =
            new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> AllChannels( params string[] specialChannels )
        {
            var allChannels = new List<string>();

            allChannels.AddRange( Channels );
            allChannels.AddRange( specialChannels );
            allChannels.AddRange( ChannelSpecific.Select( kvp => kvp.Key ) );

            foreach( var channel in allChannels.Distinct( StringComparer.OrdinalIgnoreCase ) )
            {
                yield return channel.ToLower();
            }
        }
    }
}