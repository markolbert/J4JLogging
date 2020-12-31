using System;

namespace J4JSoftware.Logging
{
    [Flags]
    public enum AvailableChannels
    {
        Console = 1 << 0,
        Debug = 1 << 1,
        File = 1 << 2,
        Twilio = 1 << 3,

        None = 0,
        Basic = Console | Debug | File,
        ConsoleDebug = Console | Debug
    }
}