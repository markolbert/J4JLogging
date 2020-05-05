namespace J4JSoftware.Logging
{
    public interface IJ4JLoggerTemplate
    {
        string GetTemplate( string baseTemplate, IJ4JLoggerConfiguration config );
    }
}