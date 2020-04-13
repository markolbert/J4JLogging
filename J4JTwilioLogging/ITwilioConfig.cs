using System.Collections.Generic;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    public interface ITwilioConfig
    {
        string AccountSID { get; set; }
        string AccountToken { get; set; }
        string FromNumber { get; set; }
        List<PhoneNumber> Recipients { get; }
    }
}