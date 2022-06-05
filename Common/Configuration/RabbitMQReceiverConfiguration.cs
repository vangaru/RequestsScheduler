namespace RequestsScheduler.Common.Configuration;

public class RabbitMQReceiverConfiguration
{
    public RabbitMQConfiguration? SourceQueueConfiguration { get; set; }
    public RabbitMQConfiguration? DestinationQueueConfiguration { get; set; }
}