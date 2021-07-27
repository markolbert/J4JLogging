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
using System.IO;
using Serilog;

namespace J4JSoftware.Logging
{
    public record TwilioParameters : ChannelParameters
    {
        private readonly string _acctSID = string.Empty;
        private readonly string _acctToken = string.Empty;
        private readonly string _fromNum = string.Empty;

        public TwilioParameters(
            J4JLogger logger )
            : base( logger )
        {
        }

        public string AccountSID
        {
            get => _acctSID;
            init => SetProperty( ref _acctSID, value );
        }

        public string AccountToken
        {
            get => _acctToken;
            init => SetProperty( ref _acctToken, value );
        }

        public string FromNumber
        {
            get => _fromNum;
            init => SetProperty( ref _fromNum, value );
        }

        public List<string> Recipients { get; } = new();
    }
}