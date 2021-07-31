using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using PostService.Data;
using PostService.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;

namespace PostService
{
    public class Program
    {
        private static IHost _host;

        public static void Main(string[] args)
        {
            _host = CreateHostBuilder(args).Build();

            ListenForIntegrationEvents();

            _host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void ListenForIntegrationEvents()
        {
            var factory = new ConnectionFactory() 
            {
                UserName = "guest",
                Password = "guest",
                HostName = "192.168.1.148",
                VirtualHost = "/",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                using var scope = _host.Services.CreateScope();
                
                var dbContext = scope.ServiceProvider.GetRequiredService<PostServiceContext>();
                
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var data = JObject.Parse(message);
                var type = ea.RoutingKey;
                if (type == "user.add")
                {
                    dbContext.Users.Add(new User()
                    {
                        ID = data["id"].Value<int>(),
                        Name = data["name"].Value<string>()
                    });
                    dbContext.SaveChanges();
                }
                else if (type == "user.update")
                {
                    var user = dbContext.Users.First(a => a.ID == data["id"].Value<int>());
                    user.Name = data["newname"].Value<string>();
                    dbContext.SaveChanges();
                }
            };

            channel.BasicConsume(
                queue: "user.postservice",
                autoAck: true,
                consumer: consumer
            );
        }
    }
}
