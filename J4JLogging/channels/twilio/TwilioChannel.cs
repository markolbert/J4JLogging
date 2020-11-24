using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    public class TwilioChannel : TextChannel
    {
        private readonly TwilioConfig _channelConfig;

        public TwilioChannel( J4JLoggerConfiguration config, TwilioConfig channelConfig )
            : base( config, channelConfig )
        {
            _channelConfig = channelConfig;

            TwilioClient.Init( _channelConfig.AccountSID, _channelConfig.AccountToken );
        }

        protected override bool ProcessLogMessage( string mesg )
        {
            var fromNumber = _channelConfig.GetFromNumber();

            _channelConfig.GetRecipients()
                .ForEach( r => MessageResource.Create(
                    body : mesg,
                    to : r,
                    from : fromNumber )
                );

            return true;
        }
    }
}