using Consumer.Application;
using Consumer.Infrastructure;
using RabbitMQLib;
using RabbitMQLib.Interfaces;

namespace Consumer.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string hostname = builder.Configuration.GetSection("RabbitMQSettings").GetSection("HostName").Value;

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure();
            builder.Services.AddRPCServerMessageBroker(hostname, "queue_rpc");
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the message broker subscription
            ConfigureRPCServerMessageBroker(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


        private static void ConfigureRPCServerMessageBroker(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var server = serviceProvider.GetRequiredService<IRPCServer>();
            var messageHandler = serviceProvider.GetRequiredService<IMessageHandler>();
            server.StartListening("queue_rpc", messageHandler);
        }

    }
}
