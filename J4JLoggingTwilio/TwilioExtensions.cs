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
using Serilog;
using Serilog.Events;
using Twilio;

namespace J4JSoftware.Logging
{
    public static class TwilioExtensions
    {
        public static J4JLoggerConfiguration AddTwilio( this J4JLoggerConfiguration loggerConfig,
                                                        TwilioConfiguration configValues,
                                                        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
                                                        string? outputTemplate = null )
        {
            if( !configValues.IsValid )
                throw new ArgumentException( "Twilio configuration values are invalid" );

            var sink = new TwilioSink( configValues.FromNumber!,
                                      configValues.Recipients!,
                                      outputTemplate ?? loggerConfig.GetOutputTemplate() );

            try
            {
                TwilioClient.Init( configValues.AccountSID!, configValues.AccountToken! );
                sink.IsConfigured = true;

                loggerConfig.AddSmsSink( sink, restrictedToMinimumLevel );
            }
            catch
            {
                sink.IsConfigured = false;
            }

            return loggerConfig;
        }
    }
}
