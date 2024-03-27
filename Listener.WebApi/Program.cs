using RabbitMQLib;
using RabbitMQLib.Interfaces;

namespace Listener.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string hostname = builder.Configuration.GetSection("RabbitMQSettings").GetSection("HostName").Value;

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddRPCClientMessageBroker(hostname);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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
    }
}
