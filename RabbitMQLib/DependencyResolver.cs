using Microsoft.Extensions.DependencyInjection;
using RabbitMQLib.Interfaces;


namespace RabbitMQLib
{
    public static class DependencyResolver
    {
        public static IServiceCollection AddRPCClientMessageBroker(this IServiceCollection services, string hostName)
        {
            services.AddSingleton<IRPCClient>(provider => new RPCClient(hostName));
            return services;
        }

        public static IServiceCollection AddRPCServerMessageBroker(this IServiceCollection services, string hostName, string queueName)
        {
            services.AddSingleton<IRPCServer>(provider => new RPCServer(hostName, queueName));
            return services;
        }
    }
}
