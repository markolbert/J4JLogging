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

using System.Collections.Generic;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;
using Twilio;

#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of TwilioChannel
    public class TwilioConfig : ChannelConfig
    {
        public string AccountSID { get; set; }
        public string AccountToken { get; set; }
        public string FromNumber { get; set; }
        public List<string> Recipients { get; set; }

        public override bool IsValid
        {
            get
            {
                if( string.IsNullOrEmpty( AccountSID ) ) return false;
                if( string.IsNullOrEmpty( AccountToken ) ) return false;
                if( string.IsNullOrEmpty( FromNumber ) ) return false;

                return Recipients.Count != 0;
            }
        }

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            TwilioClient.Init( AccountSID, AccountToken );

            return sinkConfig.Logger( lc => lc.Filter
                .ByIncludingOnly( "SendToSms" )
                .WriteTo
                .Sms<TwilioSink>(
                    new MessageTemplateTextFormatter( EnrichedMessageTemplate ),
                    FromNumber,
                    Recipients ) );
        }
    }
}