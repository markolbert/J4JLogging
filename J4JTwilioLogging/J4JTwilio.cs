using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace J4JSoftware.Logging
{
    public class J4JTwilio : IJ4JSms
    {
        public J4JTwilio( ITwilioConfig config )
        {
            if( config == null )
                throw new NullReferenceException( nameof(config) );

            if( string.IsNullOrEmpty( config.AccountToken ) )
                throw new ArgumentNullException( nameof(config.AccountToken) );

            if( string.IsNullOrEmpty( config.AccountSID ) )
                throw new ArgumentNullException( nameof(config.AccountSID) );

            if( string.IsNullOrEmpty( config.FromNumber ) )
                throw new ArgumentNullException( nameof(config.FromNumber) );

            FromNumber = new PhoneNumber( config.FromNumber );
            Recipients = config.Recipients;

            TwilioClient.Init( config.AccountSID, config.AccountToken );
        }

        public PhoneNumber FromNumber { get; }
        public List<PhoneNumber> Recipients { get; }

        public void Send( string mesg )
        {
            Recipients.ForEach( r => MessageResource.Create(
                body : mesg,
                to : r,
                from : FromNumber ) );
        }
    }
}