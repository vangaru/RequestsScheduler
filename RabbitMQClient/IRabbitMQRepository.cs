using RequestsScheduler.Common.Configuration;

namespace RequestsScheduler.RabbitMQClient;

public interface IRabbitMQRepository
{
    public void Send(string message, RabbitMQConfiguration configuration);
}