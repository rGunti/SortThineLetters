using Microsoft.Extensions.Logging;

namespace SortThineLetters.Core
{
    public abstract class LoggingService { }
    public abstract class LoggingService<TContext> : LoggingService
        where TContext : LoggingService
    {
        protected readonly ILogger<TContext> _logger;

        protected LoggingService(ILogger<TContext> logger)
        {
            _logger = logger;
        }
    }
}
