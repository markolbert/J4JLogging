using System;
using Twilio.Rest.Api.V2010.Account;

namespace J4JSoftware.Logging
{
    public class TwilioSink : SmsSink
    {
        protected override void SendMessage( string logMessage )
        {
            foreach( var rn in RecipientNumbers )
            {
                try
                {
                    MessageResource.Create( body : logMessage, to : rn, @from : FromNumber );
                }
                catch( Exception e )
                {
                    throw new InvalidOperationException(
                        $"Could not create Twilio message. Exception message was '{e.Message}'" );
                }
            }
        }
    }
}