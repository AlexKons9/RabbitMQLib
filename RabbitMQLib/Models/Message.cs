

namespace RabbitMQLib.Models
{
    public class Message
    {
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public Parameters[] Parameters { get; set; }
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }

        public Message()
        {
            
        }
        public Message(string serviceName, string methodName, Parameters[] parameters = null) 
        {
            ServiceName = serviceName;
            MethodName = methodName;
            Parameters = parameters;
            HasError = false;
        }

        public Message(string serviceName, string methodName, bool hasError, string errorDescription, Parameters[] parameters = null)
        {
            ServiceName = serviceName;
            MethodName = methodName;
            HasError = hasError;
            ErrorDescription = errorDescription;
            Parameters = parameters;
        }

    }
}
