using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Models;
using RequestsScheduler.Common.Services;
using RequestsScheduler.RabbitMQClient;

namespace RequestsScheduler.Scheduler.Services;

public class SchedulerService : ISchedulerService
{
    private readonly ISeatApplicationGenerator _seatApplicationGenerator;
    private readonly IRabbitMQRepository _rabbitMqRepository;
    private readonly RabbitMQConfiguration _rabbitMqConfiguration;

    public SchedulerService(
        ISeatApplicationGenerator seatApplicationGenerator, 
        IRabbitMQRepository rabbitMqRepository, 
        RabbitMQConfiguration rabbitMqConfiguration)
    {
        _seatApplicationGenerator = seatApplicationGenerator;
        _rabbitMqRepository = rabbitMqRepository;
        _rabbitMqConfiguration = rabbitMqConfiguration;
    }

    public void ScheduleApplicationSeat()
    {
        SeatApplication seatApplication = _seatApplicationGenerator.Generate();
        seatApplication.Status = SeatApplicationStatus.Sent.ToString();
        _rabbitMqRepository.Send(seatApplication.ToString(), _rabbitMqConfiguration);
    }
}