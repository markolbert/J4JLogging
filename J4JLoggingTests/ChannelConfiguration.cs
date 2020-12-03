﻿using System.Collections.Generic;
using J4JSoftware.Logging;

namespace J4JLoggingTests
{
    public class ChannelConfiguration : LogChannels
    {
        public ConsoleConfig Console { get; set; }
        public DebugConfig Debug { get; set; }
        public FileConfig File { get; set; }
        public TwilioTestConfig Twilio { get; set; }
        public LastEventConfig LastEvent { get; } = new LastEventConfig();

        public override IEnumerator<IJ4JChannelConfig> GetEnumerator()
        {
            yield return Console;
            yield return Debug;
            yield return File;
            yield return Twilio;
            yield return LastEvent;
        }
    }
}
