using System;

namespace J4JSoftware.Logging
{
    [Flags]
    public enum EventElements
    {
        Type = 1 << 0,
        SourceCode = 1 << 1,

        None = 0,
        All = Type | SourceCode
    }
}