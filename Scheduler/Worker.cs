using RequestsScheduler.Scheduler.Services;

namespace RequestsScheduler.Scheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly INextSeatApplicationDelayProvider _nextSeatApplicationDelayProvider;

    public Worker(ILogger<Worker> logger, INextSeatApplicationDelayProvider nextSeatApplicationDelayProvider)
    {
        _logger = logger;
        _nextSeatApplicationDelayProvider = nextSeatApplicationDelayProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(_nextSeatApplicationDelayProvider.DelayInMillis, stoppingToken);
        }
    }
}