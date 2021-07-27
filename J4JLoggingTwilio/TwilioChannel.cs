#region license

// Copyright 2021 Mark A. Olbert
// 
// This library or program 'J4JLoggingTwilio' is free software: you can redistribute it
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
using System.Reflection;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;
using Twilio;

#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of TwilioChannel
    public class TwilioChannel : Channel<TwilioParameters>
    {
        public TwilioChannel(
            J4JLogger logger )
            : base( logger )
        {

        }

        public bool IsValid
        {
            get
            {
                if( string.IsNullOrEmpty( Parameters?.AccountSID ?? string.Empty ) ) return false;
                if( string.IsNullOrEmpty( Parameters?.AccountToken ?? string.Empty ) ) return false;
                if( string.IsNullOrEmpty( Parameters?.FromNumber ?? string.Empty ) ) return false;

                return Parameters?.Recipients.Count != 0;
            }
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            if( !IsValid )
                throw new ArgumentException(
                    "Could not configure the Twilio channel because one or more required parameters required by Twilio were not defined locally" );

            TwilioClient.Init( Parameters!.AccountSID, Parameters!.AccountToken );

            return sinkConfig.Logger( lc => lc.Filter
                .ByIncludingOnly( "SendToSms" )
                .WriteTo
                .Sms<TwilioSink>(
                    new MessageTemplateTextFormatter( EnrichedMessageTemplate ),
                    Parameters!.FromNumber,
                    Parameters!.Recipients ) );
        }
    }
}