using Microsoft.Extensions.DependencyInjection;

namespace SortThineLetters.Core
{
    public static class Registration
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddSingleton<MailBoxClientManager>();
        }
    }
}
