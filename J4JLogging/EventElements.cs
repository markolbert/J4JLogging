using System;

namespace J4JSoftware.Logging
{
    // Defines the various kinds of extended information offered by IJ4JLogger
    [Flags]
    public enum EventElements
    {
        Type = 1 << 0,
        SourceCode = 1 << 1,

        None = 0,
        All = Type | SourceCode
    }
}