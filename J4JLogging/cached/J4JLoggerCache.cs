using System;
using System.Collections;
using System.Collections.Generic;
using Serilog.Events;

namespace J4JSoftware.Logging
{
    public class J4JLoggerCache : IEnumerable<CachedEntry>
    {
        private readonly List<CachedEntry> _entries = new List<CachedEntry>();

        public void Add(
            Type? loggedType,
            bool includeSms,
            LogEventLevel level,
            string template,
            string memberName,
            string sourcePath,
            int sourceLine,
            params object?[] propertyValues) =>
            _entries.Add(new CachedEntry
            {
                LoggedType = loggedType,
                IncludeSms = includeSms,
                LogEventLevel = level,
                Template = template,
                MemberName = memberName,
                SourcePath = sourcePath,
                SourceLine = sourceLine,
                PropertyValues = propertyValues
            });

        public void Clear() => _entries.Clear();

        public IEnumerator<CachedEntry> GetEnumerator()
        {
            foreach( var entry in _entries )
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}