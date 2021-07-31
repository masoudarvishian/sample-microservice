using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Entities;

namespace UserService.Services
{
    public class UserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserServiceContext _dbContext;

        public UserService(UnitOfWork unitOfWork, UserServiceContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetUsers()
        {
            // we can get all users by entity framework
            //var all = await _unitOfWork.Repository<User>().FindAll()?.ToListAsync();

            // or we can run a stored procedure
            var all = await _dbContext.Users.FromSqlRaw("spGetUsers").ToListAsync();

            return all;
        }

        public async Task UpdateUser(int id, User user)
        {
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = user.ID,
                newname = user.Name
            });

            PublishToMessageQueue("user.update", integrationEventData);
        }

        public async Task AddUser(User user)
        {
            _unitOfWork.Repository<User>().Create(user);
            await _unitOfWork.SaveChangesAsync();

            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = user.ID,
                name = user.Name
            });

            PublishToMessageQueue("user.add", integrationEventData);
        }

        private void PublishToMessageQueue(string integrationEvent, string eventData)
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

            var body = Encoding.UTF8.GetBytes(eventData);

            channel.BasicPublish(
                exchange: "user",
                routingKey: integrationEvent,
                basicProperties: null,
                body: body
            );
        }
    }
}
