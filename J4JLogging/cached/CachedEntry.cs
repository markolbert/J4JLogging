using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
#pragma warning disable 8618

namespace J4JSoftware.Logging
{
    public class CachedEntry
    {
        public Type? LoggedType { get; set; }
        public bool IncludeSms { get; set; }
        public LogEventLevel LogEventLevel { get; set; }
        public string Template { get; set; }
        public string MemberName { get; set; }
        public string SourcePath { get; set; }
        public int SourceLine { get; set; }
        public object?[] PropertyValues { get; set; }
    }
}
