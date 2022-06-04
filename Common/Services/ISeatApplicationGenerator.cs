using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Common.Services;

public interface ISeatApplicationGenerator
{
    public SeatApplication Generate();
}