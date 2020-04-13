using System;

namespace J4JSoftware.Logging
{
    [Flags]
    public enum EntryElements
    {
        Assembly = 1 << 0,
        SourceCode = 1 << 1,
        ExternalSinks = 1 << 2,

        None = 0,
        All = Assembly | SourceCode | ExternalSinks
    }
}