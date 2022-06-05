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
    private readonly RabbitMQConfiguration _configuration;
    private readonly ISeatApplicationsDbRepository _seatApplicationsDbRepository;
    private readonly IRabbitMQRepository _rabbitMqRepository;
    private ConnectionFactory? _connectionFactory;
    private IConnection? _connection;
    private IModel? _channel;

    public Worker(
        ILogger<Worker> logger, 
        RabbitMQConfiguration configuration, 
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
            UserName = _configuration.UserName,
            Password = _configuration.Password,
            VirtualHost = _configuration.VirtualHost,
            HostName = _configuration.HostName,
            DispatchConsumersAsync = true
        };
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_configuration.QueueName, durable: true, exclusive: false, autoDelete: false,
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
                await _seatApplicationsDbRepository.AddAsync(seatApplication);
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

        _channel.BasicConsume(queue: _configuration.QueueName, autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection?.Close();
    }
}