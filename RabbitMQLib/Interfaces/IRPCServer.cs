using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLib.Interfaces
{
    public interface IRPCServer
    {
        void StartListening(string queueName, IMessageHandler messageHandler);
    }
}
