using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Models;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Receiver.Repositories;

namespace RequestsScheduler.Receiver.Services;

public class ReceiverService : IReceiverService
{
    private readonly IRabbitMQRepository _rabbitMqRepository;
    private readonly ISeatApplicationsDbRepository _seatApplicationsDbRepository;
    private readonly RabbitMQConfiguration _rabbitMqConfiguration;
    private readonly ILogger<ReceiverService> _logger;

    public ReceiverService(
        IRabbitMQRepository rabbitMqRepository, 
        ISeatApplicationsDbRepository seatApplicationsDbRepository, 
        RabbitMQConfiguration rabbitMqConfiguration, 
        ILogger<ReceiverService> logger)
    {
        _rabbitMqRepository = rabbitMqRepository;
        _seatApplicationsDbRepository = seatApplicationsDbRepository;
        _rabbitMqConfiguration = rabbitMqConfiguration;
        _logger = logger;
    }

    public void Subscribe()
    {
        _rabbitMqRepository.Subscribe(Message_Received, _rabbitMqConfiguration);
    }

    private void Message_Received(object? o, BasicDeliverEventArgs eventArgs)
    {
        byte[] messageBody = eventArgs.Body.ToArray();
        string message = Encoding.UTF8.GetString(messageBody);
        var seatApplication = JsonSerializer.Deserialize<SeatApplication>(message);
        Console.WriteLine(messageBody);
        if (seatApplication == null)
        {
            _logger.LogWarning("Failed to deserialize message from queue.");
        }
        else
        {
            _seatApplicationsDbRepository.AddAsync(seatApplication);
        }
        if (o != null)
        {
            var sender = (EventingBasicConsumer) o;
            sender.Model.BasicAck(eventArgs.DeliveryTag, true);
        }
    }
}