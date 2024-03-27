using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RabbitMQLib.Interfaces;
using RabbitMQLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Newtonsoft.Json;
using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace RabbitMQLib
{
    public class RPCClient : IRPCClient, IDisposable
    {
        private readonly IConnection _connection;
        private readonly string _replyQueueName;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private TaskCompletionSource<string> _responseReceived;

        public RPCClient(string hostName)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            // Create a channel for communication
            _channel = _connection.CreateModel();

            // Generate a unique queue name for receiving RPC responses
            _replyQueueName = _channel.QueueDeclare().QueueName;

            // Create a consumer to receive RPC responses
            _consumer = new EventingBasicConsumer(_channel);

            // Set up a task completion source to track the received response
            _responseReceived = new TaskCompletionSource<string>();

            // Register an event handler for the Received event of the consumer
            _consumer.Received += (sender, args) =>
            {
                // Check if the received response's correlation ID matches the task's ID
                if (args.BasicProperties.CorrelationId == _responseReceived.Task.Id.ToString())
                {
                    // Convert the response body to a string
                    var body = args.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);

                    // Set the result of the task completion source with the response
                    _responseReceived.TrySetResult(response);
                }
            };
        }

        public Message SendRequestWithReply(Message message, string queueName)
        {
            // Generate a unique correlation ID for the request
            var correlationId = Guid.NewGuid().ToString();

            // Generate a unique queue name for receiving the request's response
            var replyQueueName = _channel.QueueDeclare().QueueName;

            // Create basic properties for the request
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            // Convert the request object to JSON and encode it as bytes
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore, // Ignore default values during serialization
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            var json = JsonConvert.SerializeObject(message, settings);
            var messageBytes = Encoding.UTF8.GetBytes(json);

            // Set up a task completion source to track the received response for this request
            var responseReceived = new TaskCompletionSource<Message>();

            // Create a new consumer to receive the response
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, args) =>
            {
                // Check if the received response's correlation ID matches the request's correlation ID
                if (args.BasicProperties.CorrelationId == correlationId)
                {
                    // Convert the response body to a string
                    var response = JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(args.Body.ToArray()));

                    // Set the result of the task completion source with the response
                    responseReceived.SetResult(response);
                }
            };

            // Start consuming messages from the reply queue
            _channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            // Publish the request message to the RPC queue with the specified properties and body
            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: messageBytes);

            // Wait for the response task to complete and return the received response
            return responseReceived.Task.GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
