using RequestsScheduler.Common.Models;
using RequestsScheduler.Receiver.Data;

namespace RequestsScheduler.Receiver.Repositories;

public sealed class SeatApplicationsDbRepository : ISeatApplicationsDbRepository, IDisposable
{
    private readonly ReceiverContext _receiverContext;

    public SeatApplicationsDbRepository(ReceiverContext receiverContext)
    {
        _receiverContext = receiverContext;
    }

    public async Task AddAsync(SeatApplication seatApplication)
    {
        if (_receiverContext.SeatApplications == null)
        {
            throw new ApplicationException("Invalid Receiver Context.");
        }
        
        await _receiverContext.SeatApplications.AddAsync(seatApplication);
        await _receiverContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _receiverContext.Dispose();
    }
}