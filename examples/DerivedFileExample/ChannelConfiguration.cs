using System.Collections.Generic;
using J4JSoftware.Logging;
#pragma warning disable 8618

namespace J4JLogger.Examples
{
    public class ChannelConfiguration : LogChannels
    {
        public ConsoleConfig Console { get; set; }
        public DebugConfig Debug { get; set; }
        public FileConfig File { get; set; }

        public override IEnumerator<IChannelConfig> GetEnumerator()
        {
            yield return Console;
            yield return Debug;
            yield return File;
        }
    }
}
