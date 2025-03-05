using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using AljasAuthApi.Config;

namespace AljasAuthApi.Services
{
    public class RabbitMQService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public RabbitMQService(IOptions<RabbitMQSettings> rabbitConfig)
        {
            try
            {
                var config = rabbitConfig.Value;
                var factory = new ConnectionFactory()
                {
                    HostName = config.HostName,
                    Port = config.Port,
                    UserName = config.UserName,
                    Password = config.Password
                };

                _exchangeName = config.ExchangeName;

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // ✅ Declare Exchange (No Queue)
                _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);

                Console.WriteLine("[RabbitMQ] Connected successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Connection Error: {ex.Message}");
                throw;
            }
        }

        public void PublishMessage(string routingKey, object message)
        {
            if (_channel == null)
            {
                Console.WriteLine("[RabbitMQ] Channel not initialized!");
                return;
            }

            var messageBody = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"[RabbitMQ] Message Published to Exchange: {_exchangeName}, RoutingKey: {routingKey}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
