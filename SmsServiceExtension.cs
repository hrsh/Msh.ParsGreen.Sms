using Microsoft.Extensions.DependencyInjection;

namespace ParsGreen.HttpService.Core.Api
{
    public static class SmsServiceExtension
    {
        public static IServiceCollection AddParsGreen(this IServiceCollection service)
        {
            service.AddScoped<ISmsProvider, SmsProvider>();
            return service;
        }
    }
}