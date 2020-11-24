using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Twilio.Types;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    // Base class for containing the information needed to configure an instance of TwilioChannel
    public class TwilioConfig : J4JChannelConfig
    {
        public string AccountSID { get; set; }
        public string AccountToken { get; set; }
        public string FromNumber { get; set; }
        public List<string> Recipients { get; set; }

        public PhoneNumber? GetFromNumber()
        {
            try
            {
                return new PhoneNumber( FromNumber );
            }
            catch
            {
                return null;
            }
        }

        public List<PhoneNumber> GetRecipients()
        {
            try
            {
                return Recipients.Select( r => new PhoneNumber( r ) ).ToList();
            }
            catch
            {
                return new List<PhoneNumber>();
            }
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