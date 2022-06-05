using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Receiver.Repositories;

public interface ISeatApplicationsDbRepository
{
    public Task AddAsync(SeatApplication seatApplication);
}