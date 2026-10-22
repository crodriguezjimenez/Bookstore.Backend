namespace OrderService.Application.Common.Interfaces;

public interface IMessageBusPublisher
{
    Task PublishAsync<T>(T message, string queueName) where T : class;
}


