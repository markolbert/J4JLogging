using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    [Channel("Twilio")]
    public class TwilioChannel : TextChannel<ITwilioConfig>
    {
        private ITwilioConfig _config;
        private bool _initialized;

        public override bool Initialize( ITwilioConfig config )
        {
            _initialized = false;

            if( config == null || !config.IsValid )
                return false;

            _config = config;

            TwilioClient.Init(_config.AccountSID, _config.AccountToken);
            _initialized = true;

            return true;
        }

        protected override bool ProcessLogMessage( string mesg )
        {
            if( _config == null )
                return false;

            var fromNumber = _config.GetFromNumber();

            _config.GetRecipients()
                .ForEach( r => MessageResource.Create(
                    body : mesg,
                    to : r,
                    from : fromNumber )
                );

            return true;
        }
    }
}