using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Configuration;

namespace J4JSoftware.Logging
{
    public static class SinkExtensions
    {
        public static LoggerConfiguration Sms<TSmsSink>( 
            this LoggerSinkConfiguration sinkConfig, 
            string fromNumber,
            IEnumerable<string> recipientNumbers )
            where TSmsSink : SmsSink, new()
        {
            return sinkConfig.Sink( new TSmsSink
            {
                FromNumber = fromNumber,
                RecipientNumbers = recipientNumbers.ToList()
            } );
        }

        public static LoggerConfiguration LastEvent( this LoggerSinkConfiguration sinkConfig, EventHandler<string> handler )
        {
            var sink = new LastEventSink();
            sink.LogEvent += handler;

            return sinkConfig.Sink( sink );
        }
    }
}