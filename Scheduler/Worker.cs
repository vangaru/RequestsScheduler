using RequestsScheduler.Common.Configuration;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Scheduler.Services;

namespace RequestsScheduler.Scheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly INextSeatApplicationDelayProvider _nextSeatApplicationDelayProvider;
    private readonly RabbitMQConfiguration _rabbitMqConfiguration;
    private readonly IRabbitMQRepository _rabbitMqRepository;

    public Worker(
        ILogger<Worker> logger, 
        INextSeatApplicationDelayProvider nextSeatApplicationDelayProvider, 
        RabbitMQConfiguration rabbitMqConfiguration, 
        IRabbitMQRepository rabbitMqRepository)
    {
        _logger = logger;
        _nextSeatApplicationDelayProvider = nextSeatApplicationDelayProvider;
        _rabbitMqConfiguration = rabbitMqConfiguration;
        _rabbitMqRepository = rabbitMqRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            // TODO: Generate real seat application.
            int delay = _nextSeatApplicationDelayProvider.DelayInMillis;
            _rabbitMqRepository.Send(delay.ToString(), _rabbitMqConfiguration);
            await Task.Delay(delay, stoppingToken);
        }
    }
}