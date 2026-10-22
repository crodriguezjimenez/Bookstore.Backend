using Microsoft.Extensions.Configuration;
using OrderService.Application.Common.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessageBusPublisher, IDisposable
{
    private readonly IConnection? _connection;

    private readonly RabbitMQ.Client.IModel? _channel;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        try
        {
            var amqpUri = configuration["RabbitMQ:Uri"];
            if (string.IsNullOrEmpty(amqpUri))
            {
                throw new ArgumentNullException("La URI de RabbitMQ no está configurada en appsettings.json.");
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(amqpUri),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[RabbitMQ Publisher Error] No se pudo conectar a CloudAMQP: {ex.Message}");
            Console.ResetColor();
        }
    }

    public Task PublishAsync<T>(T message, string queueName) where T : class
    {

        if (_channel is null)
        {
            Console.WriteLine("[RabbitMQ Publisher] Canal no inicializado. El evento no se enviará.");
            return Task.CompletedTask;
        }

        _channel.QueueDeclare(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

       
        var jsonPayload = JsonSerializer.Serialize(message);
        var bodyBytes = Encoding.UTF8.GetBytes(jsonPayload);

        
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
                
        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: properties,
            body: bodyBytes);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[RabbitMQ Publisher] Evento publicado exitosamente en la cola '{queueName}': {jsonPayload}");
        Console.ResetColor();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
