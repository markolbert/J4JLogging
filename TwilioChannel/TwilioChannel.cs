using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    [Channel("Twilio")]
    public class TwilioChannel : TextChannel<ITwilioConfig>
    {
        private ITwilioConfig _config;

        public TwilioChannel()
        {
        }

        public TwilioChannel(IConfigurationRoot configRoot, string loggerSection = "Logger")
            : base(configRoot, loggerSection)
        {
        }

        public override bool Initialize( ITwilioConfig config )
        {
            if( config == null || !config.IsValid )
                return false;

            _config = config;

            TwilioClient.Init(_config.AccountSID, _config.AccountToken);

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