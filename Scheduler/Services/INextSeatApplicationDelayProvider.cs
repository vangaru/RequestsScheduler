namespace RequestsScheduler.Scheduler.Services;

public interface INextSeatApplicationDelayProvider
{
    public int DelayInMillis { get; }
}