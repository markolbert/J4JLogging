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

        public override LoggerConfiguration Configure( LoggerSinkConfiguration sinkConfig )
        {
            TwilioClient.Init(AccountSID, AccountToken);

            return sinkConfig.Logger( lc => lc.Filter
                .ByIncludingOnly( "SendToSms" )
                .WriteTo
                .Sms<TwilioSink>( 
                    new MessageTemplateTextFormatter( EnrichedMessageTemplate ), 
                    FromNumber,
                    Recipients ) );
        }

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
    }
}