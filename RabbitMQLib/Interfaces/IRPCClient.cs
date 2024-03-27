using RabbitMQLib.Models;


namespace RabbitMQLib.Interfaces
{
    public interface IRPCClient
    {
        Message SendRequestWithReply(Message message, string queueName);
    }
}
