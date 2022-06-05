using Microsoft.EntityFrameworkCore;
using RequestsScheduler.Common.Configuration;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Receiver;
using RequestsScheduler.Receiver.Data;
using RequestsScheduler.Receiver.Repositories;
using RequestsScheduler.Receiver.Services;

const string rabbitMQConfigurationSection = "RabbitMQConfiguration";
const string connectionString = "ReceiverConnectionString";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        var rabbitMqConfiguration = configuration
            .GetSection(rabbitMQConfigurationSection)
            .Get<RabbitMQConfiguration>();

        services.AddSingleton(rabbitMqConfiguration);
        
        var dbContextOptions = new DbContextOptionsBuilder<ReceiverContext>();
        dbContextOptions.UseNpgsql(configuration.GetConnectionString(connectionString));
        services.AddSingleton(new ReceiverContext(dbContextOptions.Options));

        services.AddSingleton<ISeatApplicationsDbRepository, SeatApplicationsDbRepository>();
        services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
        services.AddSingleton<IReceiverService, ReceiverService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();