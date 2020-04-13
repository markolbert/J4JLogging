using System.Collections.Generic;
using System.Linq;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    public class TwilioConfig : ITwilioConfig
    {
        public string AccountSID { get; set; }
        public string AccountToken { get; set; }
        public string FromNumber { get; set; }
        public List<string> Recipients { get; set; }

        public PhoneNumber GetFromNumber()
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

        public bool IsValid
        {
            get
            {
                if( string.IsNullOrEmpty( AccountSID ) ) return false;
                if( string.IsNullOrEmpty( AccountToken ) ) return false;
                if( string.IsNullOrEmpty( FromNumber ) ) return false;
                if( Recipients == null || Recipients.Count == 0 ) return false;

                return true;
            }
        }
    }
}