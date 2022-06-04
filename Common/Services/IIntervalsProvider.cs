using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Common.Services;

public interface IIntervalsProvider
{
    public Interval CurrentInterval { get; }
    public IEnumerable<Interval> Intervals { get; }
}