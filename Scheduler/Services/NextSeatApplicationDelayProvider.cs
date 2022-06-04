using RequestsScheduler.Common.Models;
using RequestsScheduler.Common.Services;

namespace RequestsScheduler.Scheduler.Services;

public class NextSeatApplicationDelayProvider : INextSeatApplicationDelayProvider
{
    private const int DefaultDelay = 10_000;
    private const int MillisInHour = 3_600_000;
    
    private readonly IIntervalsProvider _intervalsProvider;

    public NextSeatApplicationDelayProvider(IIntervalsProvider intervalsProvider)
    {
        _intervalsProvider = intervalsProvider;
    }

    public int DelayInMillis
    {
        get
        {
            Interval currentInterval = _intervalsProvider.CurrentInterval;
            var random = new Random();
            int seatApplicationCount = random.Next(currentInterval.MinSeatApplicationsCount, currentInterval.MaxSeatApplicationsCount);
            try
            {
                return MillisInHour / seatApplicationCount;
            }
            catch (DivideByZeroException)
            {
                return DefaultDelay;
            }
        }
    }
}