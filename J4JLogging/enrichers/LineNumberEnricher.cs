using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J4JSoftware.Logging
{
    public class LineNumberEnricher : BaseEnricher
    {
        public LineNumberEnricher()
            : base("SourceLineNumber")
        {
        }

        protected override bool EnrichContext => LineNumber > 0;
        protected override object GetValue() => LineNumber;

        public int LineNumber { get; set; }
    }
}
