using System.Collections;
using System.Collections.Generic;

namespace J4JSoftware.Logging
{
    public abstract class LogChannels : ILogChannels
    {
        protected LogChannels()
        {
        }

        public abstract IEnumerator<IJ4JChannelConfig> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
