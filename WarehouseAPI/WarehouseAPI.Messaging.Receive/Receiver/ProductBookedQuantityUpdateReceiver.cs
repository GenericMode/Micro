using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WarehouseAPI.Messaging.Receive.Options;
using WarehouseAPI.Service.Models;
using WarehouseAPI.Service.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;




namespace WarehouseAPI.Messaging.Receive.Receiver
{
    public class ProductBookedQuantityUpdateReceiver : BackgroundService
    {
        private IChannel _channel;
        private IConnection _connection;
        private readonly IProductBookedQuantityUpdateService _productBookedQuantityUpdateService;
        private readonly ILogger<ProductBookedQuantityUpdateReceiver> _logger;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public ProductBookedQuantityUpdateReceiver(IProductBookedQuantityUpdateService productBookedQuantityUpdateService, IOptions<RabbitMqConfiguration> rabbitMqOptions,ILogger<ProductBookedQuantityUpdateReceiver> logger)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _username = rabbitMqOptions.Value.UserName;
            _password = rabbitMqOptions.Value.Password;
            _productBookedQuantityUpdateService = productBookedQuantityUpdateService;
            _logger = logger;
            InitializeRabbitMqListener();
        }

        private async Task InitializeRabbitMqListener()
        {
            try 
                {
                var factory = new ConnectionFactory
                            {
                                HostName = _hostname,
                                UserName = _username,
                                Password = _password
                            };

                            _connection = await factory.CreateConnectionAsync();
                            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
                            _channel = await _connection.CreateChannelAsync();

                            if (_channel == null)
                                {
                                    Console.WriteLine("Error: _channel is null after calling CreateChannelAsync.");
                                    throw new InvalidOperationException("Channel creation failed.");
                                }
                                else
                                {
                                    Console.WriteLine("Channel created successfully.");
                                }

                            await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                            
                            Console.WriteLine("✅ ReceiveListener was initialized successfully!");
                            _logger.LogInformation("Initializing RabbitMQ listener...");
                }

             catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during RabbitMQ listener initialization.");
                    throw;
                }


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMqReceiver starting ExecuteAsync.");
            // Await the initialization of the connection and channel.
            await InitializeRabbitMqListener();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var productBookedQuantityUpdateModel = JsonConvert.DeserializeObject<ProductBookedQuantityUpdateModel>(content);

                HandleMessage(productBookedQuantityUpdateModel);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
                Console.WriteLine("✅ Receive message was handled");
            };
            consumer.ShutdownAsync += OnConsumerShutdown;
            consumer.RegisteredAsync += OnConsumerRegistered;
            consumer.UnregisteredAsync += OnConsumerUnregistered;
            //consumer.ConsumerCancelled += OnConsumerCancelled;

            if (string.IsNullOrEmpty(_queueName))
            {
                Console.WriteLine("Error: _queueName is null or empty.");
                throw new InvalidOperationException("Queue name is not set.");
            }
            else
            {
                Console.WriteLine($"Using queue: {_queueName}");
            }

            try
            {
                    string consumerTag = await _channel.BasicConsumeAsync(_queueName, false, consumer);
                    _logger.LogInformation("Consumer started with tag: {ConsumerTag}", consumerTag);

            }
            catch (Exception ex)
            {
                    _logger.LogError(ex, "Error starting consumer.");
                    throw;
            }

        }

        private void HandleMessage(ProductBookedQuantityUpdateModel productBookedQuantityUpdateModel)
        {
            _productBookedQuantityUpdateService.UpdateProductQuantityInProducts(productBookedQuantityUpdateModel);
        }

        private Task OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
                Console.WriteLine("Consumer cancelled.");
                return Task.CompletedTask;
        }

        private Task OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
                Console.WriteLine("Consumer unregistered.");
                return Task.CompletedTask;
        }

        private Task OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
                Console.WriteLine("Consumer registered.");
                return Task.CompletedTask;
        }

        private Task OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
                Console.WriteLine("Consumer shutdown.");
                return Task.CompletedTask;
        }

        private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
                Console.WriteLine("RabbitMQ connection is shutdown.");
                return Task.CompletedTask;
        }

        public override void Dispose()
        {
            try
            {
            _channel.CloseAsync();
            _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while disposing RabbitMQ resources.");
            }
            base.Dispose();
        }
    }
}