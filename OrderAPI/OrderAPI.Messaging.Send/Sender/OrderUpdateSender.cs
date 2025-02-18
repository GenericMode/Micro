using System;
using System.Threading.Tasks;
using System.Text;
using OrderAPI.Domain.Entities;
using OrderAPI.Messaging.Send.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonConverter.Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text.Json.Serialization;

namespace OrderAPI.Messaging.Send.Sender
{
    public class OrderUpdateSender : IOrderUpdateSender
    {
        private readonly string _hostname;
        private readonly string _password;
        private readonly string _queueName;
        private readonly string _username;
        private IConnection _connection;
        private IChannel _channel;
        private readonly RabbitMqConfiguration _config;

        

        public OrderUpdateSender(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            if (rabbitMqOptions == null || rabbitMqOptions.Value == null)
                    {
                        throw new ArgumentNullException(nameof(rabbitMqOptions), "RabbitMqConfiguration is null!");
                    }
             
             _config = rabbitMqOptions?.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
                Console.WriteLine("✅ OrderUpdateSender created successfully!");

            _queueName = rabbitMqOptions.Value.QueueName;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;

           Task.Run(() => CreateConnection());
        }

        public async Task SendOrder(Order order)
        {
            if (await ConnectionExists())
            {
                using (_channel = await _connection.CreateChannelAsync())
                {
                    await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonConvert.SerializeObject(order);
                    var body = Encoding.UTF8.GetBytes(json);

                    await _channel.BasicPublishAsync(exchange: "", routingKey: _queueName, true, body: body);
                    Console.WriteLine($"Order was sent {body}");

                }
            }
        }

        private async Task CreateConnection()
        {
            try
            {
                Console.WriteLine("Attempting to create RabbitMQ connection...");
                var factory = new ConnectionFactory
                {
                    HostName = _hostname,
                    UserName = _username,
                    Password = _password
                };
                _connection = await factory.CreateConnectionAsync();
                Console.WriteLine("RabbitMQ connection created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private async Task<bool> ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            await CreateConnection();

            return _connection != null;
        }
    }
}