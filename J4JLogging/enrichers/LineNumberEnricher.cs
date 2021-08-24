namespace J4JSoftware.Logging
{
    public class LineNumberEnricher : BaseEnricher
    {
        public LineNumberEnricher()
            : base("SourceLineNumber")
        {
        }

        public override bool EnrichContext => LineNumber > 0;
        public override object GetValue() => $"#{LineNumber}";

        public int LineNumber { get; set; }
    }
}
