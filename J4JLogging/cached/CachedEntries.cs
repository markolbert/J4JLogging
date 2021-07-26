using System;
using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public record CachedEntries(
        Type? LoggedType,
        bool IncludeSourcePath,
        string? SourcePathRoot,
        bool OutputMultiLineEvents,
        bool OutputToSms
    )
    {
        public List<CachedEntry> Entries { get; } = new();

        public void Add(
            LogEventLevel level,
            string template,
            string memberName,
            string sourcePath,
            int sourceLine,
            params object[] propertyValues)
        {
            Entries.Add(new CachedEntry
                (
                    level,
                    template,
                    memberName,
                    sourcePath,
                    sourceLine,
                    propertyValues
                )
            );
        }


    };
}