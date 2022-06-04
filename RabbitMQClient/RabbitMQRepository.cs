using System.Text;
using RabbitMQ.Client;
using RequestsScheduler.Common.Configuration;

namespace RequestsScheduler.RabbitMQClient;

public class RabbitMQRepository : IRabbitMQRepository
{
    public void Send(string message, RabbitMQConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration.HostName,
            UserName = configuration.UserName,
            Password = configuration.Password,
            Port = configuration.Port,
            VirtualHost = configuration.VirtualHost
        };

        using IConnection connection = factory.CreateConnection();
        
        using IModel channel = connection.CreateModel();
        channel.QueueDeclare(
            queue: configuration.QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: false, 
            arguments: null);
        
        IBasicProperties channelProperties = channel.CreateBasicProperties();

        byte[] messageBody = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "", 
            routingKey: configuration.QueueName, 
            basicProperties: channelProperties, 
            body: messageBody);
    }
}