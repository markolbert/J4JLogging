namespace J4JSoftware.Logging
{
    [J4JEnricher("SourceLineNumber")]
    public class LineNumberEnricher : BaseEnricher
    {
        public override bool EnrichContext => LineNumber > 0;
        public override object GetValue() => $"#{LineNumber}";

        public int LineNumber { get; set; }
    }
}
