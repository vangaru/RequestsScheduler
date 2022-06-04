using RequestsScheduler.Scheduler.Services;

namespace RequestsScheduler.Scheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly INextSeatApplicationDelayProvider _nextSeatApplicationDelayProvider;
    private readonly ISchedulerService _scheduler;

    public Worker(
        ILogger<Worker> logger, 
        INextSeatApplicationDelayProvider nextSeatApplicationDelayProvider, 
        ISchedulerService scheduler)
    {
        _logger = logger;
        _nextSeatApplicationDelayProvider = nextSeatApplicationDelayProvider;
        _scheduler = scheduler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _scheduler.ScheduleApplicationSeat();
            await Task.Delay(_nextSeatApplicationDelayProvider.DelayInMillis, stoppingToken);
        }
    }
}