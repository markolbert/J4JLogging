using J4JSoftware.Logging;

namespace J4JLoggingTests
{
    public interface ICompositionRoot
    {
        IJ4JLogger J4JLogger { get; }
        LastEventConfig LastEventConfig { get; }
    }
}