using RabbitMQ.Client.Events;
using RequestsScheduler.Common.Configuration;

namespace RequestsScheduler.RabbitMQClient;

public interface IRabbitMQRepository
{
    public void Send(string message, RabbitMQConfiguration configuration);
    public void Subscribe(EventHandler<BasicDeliverEventArgs> eventHandler, RabbitMQConfiguration configuration);
}