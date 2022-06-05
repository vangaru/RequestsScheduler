using Microsoft.EntityFrameworkCore;
using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Receiver.Data;

public class ReceiverContext : DbContext
{
    public DbSet<SeatApplication>? SeatApplications { get; set; }

    public ReceiverContext(DbContextOptions<ReceiverContext> options) : base(options)
    {
    }
}