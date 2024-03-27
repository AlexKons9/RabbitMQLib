using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQLib.Interfaces;
using RabbitMQLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQLib
{
    public class RPCServer : IRPCServer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RPCServer(string hostName, string queueName)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            // Create a channel for communication
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public void StartListening(string queueName, IMessageHandler messageHandler)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) =>
            {
                Message response = null;

                var body = args.Body.ToArray();
                var request = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(body));
                var correlationId = args.BasicProperties.CorrelationId;

                try
                {
                    // Process the request and generate the response
                    response = await messageHandler.Handle(request);
                }
                catch (Exception ex)
                {
                    response = new Message(String.Empty, String.Empty, true, "Error: " + ex.Message);
                }
                finally
                {
                    var replyProperties = _channel.CreateBasicProperties();
                    replyProperties.CorrelationId = correlationId;

                    var settings = new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    };

                    var json = JsonConvert.SerializeObject(response, settings);
                    var responseBytes = Encoding.UTF8.GetBytes(json);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: args.BasicProperties.ReplyTo,
                        basicProperties: replyProperties,
                        body: responseBytes);

                    _channel.BasicAck(
                        deliveryTag: args.DeliveryTag,
                        multiple: false);
                }
            };
            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
        }
        
        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
