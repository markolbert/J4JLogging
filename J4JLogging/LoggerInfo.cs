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
using System.Collections.Generic;
using System.Linq;
using J4JSoftware.Logging;
using Microsoft.Extensions.Configuration;

namespace J4JSoftware.Logging
{
    public class LoggerInfo
    {
        public static LoggerInfo Create(IConfiguration config)
        {
            var retVal = new LoggerInfo
            {
                Global = config.GetSection( nameof(Global) ).Get<ChannelParameters>() ?? new ChannelParameters( null ),
                Channels = config.GetSection( nameof(Channels) ).Get<List<string>>() ?? new List<string>(),
                ChannelSpecific = config.GetSection( nameof(ChannelSpecific) )
                    .Get<Dictionary<string, ChannelParameters>>() ?? new Dictionary<string, ChannelParameters>()
            };

            // make sure all the channels specified in ChannelSpecific also appear in Channels
            foreach (var kvp in retVal.ChannelSpecific)
            {
                if (!retVal.Channels.Any(x => x.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase)))
                    retVal.Channels.Add(kvp.Key);
            }

            return retVal;
        }

        public LoggerInfo()
        {
            Global = new ChannelParameters( null );
            Channels = new List<string>();
            ChannelSpecific = new Dictionary<string, ChannelParameters>();
        }

        public ChannelParameters Global { get; set; }
        public List<string> Channels { get; set; }
        public Dictionary<string, ChannelParameters> ChannelSpecific { get; set; }
    }
}