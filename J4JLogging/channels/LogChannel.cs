using System;

namespace J4JSoftware.Logging
{
    [Flags]
    public enum LogChannel
    {
        Console = 1 << 0,
        Debug = 1 << 1,
        File = 1 << 2,
        TextWriter = 1 << 3,

        None = 0,
        All = Console | Debug | File | TextWriter
    }
}