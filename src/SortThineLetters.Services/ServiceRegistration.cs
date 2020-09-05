using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SortThineLetters.Services.Services;
using SortThineLetters.Services.Services.Impl;

namespace SortThineLetters.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddAutoMapper(typeof(ServiceRegistration))
                .AddTransient<IMailBoxService, MailBoxService>();
        }
    }
}
