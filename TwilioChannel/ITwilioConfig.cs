using System.Collections.Generic;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    public interface ITwilioConfig
    {
        string AccountSID { get; }
        string AccountToken { get; }
        string FromNumber { get; }
        List<string> Recipients { get; }

        bool IsValid { get; }

        PhoneNumber GetFromNumber();
        List<PhoneNumber> GetRecipients();
    }
}