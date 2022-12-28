// Copyright (c) 2021, 2022 Mark A. Olbert 
// 
// This file is part of J4JLogger.
//
// J4JLogger is free software: you can redistribute it and/or modify it 
// under the terms of the GNU General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
// J4JLogger is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
// 
// You should have received a copy of the GNU General Public License along 
// with J4JLogger. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using Twilio.Rest.Api.V2010.Account;

namespace J4JSoftware.Logging
{
    public class TwilioSink : SmsSink
    {
        public TwilioSink( string fromNumber,
                           IEnumerable<string> recipientNumbers,
                           string outputTemplate )
            : base( outputTemplate )
        {
            FromNumber = fromNumber;
            RecipientNumbers = recipientNumbers.ToList();
        }

        public string FromNumber { get; }
        public List<string> RecipientNumbers { get; }
        public bool IsConfigured { get; internal set; }

        protected override void SendMessage( string logMessage )
        {
            if( !IsConfigured )
                throw new ArgumentException( $"{nameof( TwilioSink )} is not configured" );

            foreach( var rn in RecipientNumbers! )
            {
                try
                {
                    MessageResource.Create( body: logMessage, to: rn, @from: FromNumber );
                }
                catch( Exception e )
                {
                    throw new
                        InvalidOperationException( $"Could not create Twilio message. Exception message was '{e.Message}'" );
                }
            }
        }
    }
}
