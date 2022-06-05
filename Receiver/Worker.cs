using System.Globalization;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Models;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Receiver.Repositories;

namespace RequestsScheduler.Receiver;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly RabbitMQReceiverConfiguration _configuration;
    private readonly ISeatApplicationsDbRepository _seatApplicationsDbRepository;
    private readonly IRabbitMQRepository _rabbitMqRepository;
    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public Worker(
        ILogger<Worker> logger, 
        RabbitMQReceiverConfiguration configuration, 
        ISeatApplicationsDbRepository seatApplicationsDbRepository, 
        IRabbitMQRepository rabbitMqRepository)
    {
        _logger = logger;
        _configuration = configuration;
        _seatApplicationsDbRepository = seatApplicationsDbRepository;
        _rabbitMqRepository = rabbitMqRepository;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _connectionFactory = new ConnectionFactory
        {
            UserName = _configuration.SourceQueueConfiguration?.UserName,
            Password = _configuration.SourceQueueConfiguration?.Password,
            VirtualHost = _configuration.SourceQueueConfiguration?.VirtualHost,
            HostName = _configuration.SourceQueueConfiguration?.HostName,
            DispatchConsumersAsync = true
        };
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_configuration.SourceQueueConfiguration?.QueueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null);
        _channel.BasicQos(0, 1, false);
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            byte[] messageBody = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(messageBody);
            try
            {
                var seatApplication = JsonSerializer.Deserialize<SeatApplication>(message);
                if (seatApplication == null)
                {
                    throw new JsonException();
                }

                seatApplication.Status = SeatApplicationStatus.Received.ToString();
                seatApplication.DateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                await _seatApplicationsDbRepository.AddAsync(seatApplication);

                string seatApplicationJson = JsonSerializer.Serialize(seatApplication);
                if (_configuration.DestinationQueueConfiguration == null)
                {
                    throw new ApplicationException("RabbitMQ configuration section is incorrectly configured.");
                }
                _rabbitMqRepository.Send(seatApplicationJson, _configuration.DestinationQueueConfiguration);
            }
            catch (JsonException)
            {
                _logger.LogWarning("Failed to deserialize {message}", message);
            }
            catch (AlreadyClosedException)
            {
                _logger.LogError("RabbitMQ is closed.");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(default, e, e.Message);
                throw;
            }
            _channel?.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _configuration.SourceQueueConfiguration?.QueueName, autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection?.Close();
    }
}