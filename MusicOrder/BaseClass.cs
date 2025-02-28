using Serilog;

namespace MusicOrder
{
    public abstract class BaseClass
    {
        protected static readonly ILogger _logger = Log.ForContext(typeof(BaseClass));
    }
}