using Microsoft.Extensions.DependencyInjection;
using RabbitMQLib.Interfaces;
using Consumer.Application.Handlers;
using Consumer.Application.Services;

namespace Consumer.Application
{
    public static class DependencyResolver
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IMessageHandler, MessageHandler>();
            services.AddScoped<BaseService, SomeService>();
            return services;
        }
    }
}
