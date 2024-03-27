using Consumer.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQLib.Interfaces;
using RabbitMQLib.Models;
using Newtonsoft.Json;
using Consumer.Domain.Models;

namespace Consumer.Application.Handlers
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IServiceScopeFactory _provider;

        public MessageHandler(IServiceScopeFactory provider)
        {
            _provider = provider;
        }

        public async Task<Message> Handle(Message message)
        {
            try
            {
                string serviceName = message.ServiceName;
                string methodName = message.MethodName;

                using (var scope = _provider.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    // Assuming your services implement a common interface IService
                    var service = serviceProvider.GetServices<BaseService>()
                        .FirstOrDefault(s => s.GetType().Name == serviceName);

                    if (service != null)
                    {
                        var methodInfo = service.GetType().GetMethod(methodName);

                        if (methodInfo != null)
                        {
                            var parameters = methodInfo.GetParameters()
                                .Select(param =>
                                {
                                    var parameterValue = message.Parameters
                                        .FirstOrDefault(p => p.Name == param.Name)?.Value;

                                    if (parameterValue != null)
                                    {
                                        if(typeof(BaseModel).IsAssignableFrom(param.ParameterType))
                                        {
                                            return JsonConvert.DeserializeObject(parameterValue.ToString(), param.ParameterType, new JsonSerializerSettings());
                                        }
                                        return Convert.ChangeType(parameterValue, param.ParameterType);
                                    }

                                    return null;
                                }).ToArray();

                            // Invoke the method on the service instance
                            var result = methodInfo.Invoke(service, parameters);
                            if (result == null)
                                return new Message(serviceName, methodName);

                            return new Message(serviceName, methodName, new Parameters[] { new Parameters { Name = "Result", Value = result } });
                        }
                    }
                }

                // Service or method not found so return message with hasError true and with Description
                return new Message(serviceName, methodName, true, "Service or method not found.");
            }
            catch (Exception ex)
            {
                return new Message(message.ServiceName, message.MethodName, true, ex.Message);
            }
        }
    }
}
