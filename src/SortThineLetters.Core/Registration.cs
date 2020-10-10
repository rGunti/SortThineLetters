using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SortThineLetters.Core.Mapping;

namespace SortThineLetters.Core
{
    public static class Registration
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            return services
                .AddAutoMapper(typeof(MailKitMappingProfile))
                .AddSingleton<MailBoxClientManager>();
        }
    }
}
