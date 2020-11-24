using System.Collections.Generic;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    // Defines the functionality of a type which can be used to configure an instance of TwilioChannel
    public interface ITwilioConfig : IChannelConfig<TwilioChannel>
    {
        string AccountSID { get; }
        string AccountToken { get; }
        string FromNumber { get; }
        List<string> Recipients { get; }

        // Flag indicating whether the instance is properly configured
        bool IsValid { get; }

        PhoneNumber? GetFromNumber();
        List<PhoneNumber> GetRecipients();
    }
}