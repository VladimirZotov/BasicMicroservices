using PlatformService.Dto;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private const string EXCHANGE_TITLE = "trigger";

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            Console.WriteLine($"--> Connecting to RabbitMq with params {_configuration["RabbitMq:Host"]}:{_configuration["RabbitMq:Port"]}");

            var factory = new ConnectionFactory() { 
                HostName = _configuration["RabbitMq:Host"],
                Port = int.Parse(_configuration["RabbitMq:Port"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: EXCHANGE_TITLE, type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine($"--> Connected to message bus");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
            }

        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {

            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection Closed, not sending message...");
            }
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus disposed");
            if (_channel.IsOpen)
            { 
                _channel.Close();
                _connection.Close();
            }
        }

        private void SendMessage(string message)
        { 
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: EXCHANGE_TITLE, routingKey: string.Empty, basicProperties: null, body: body);

            Console.WriteLine($"--> We have sent {message}");
        }
    }
}
