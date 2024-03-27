using RabbitMQLib.Models;


namespace RabbitMQLib.Interfaces
{
    public interface IMessageHandler
    {
        Task<Message> Handle(Message message);
    }
}
