using Microsoft.EntityFrameworkCore;
using RequestsScheduler.Common.Configuration;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Receiver;
using RequestsScheduler.Receiver.Data;
using RequestsScheduler.Receiver.Repositories;
using RequestsScheduler.Receiver.Services;

const string rabbitMQReceiverConfigurationSection = "RabbitMQReceiverConfiguration";
const string connectionString = "ReceiverConnectionString";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        AppContext.SetSwitch("Npsql.EnableLegacyTimestampBehavior", true);
        
        IConfiguration configuration = hostContext.Configuration;

        var rabbitMqReceiverConfiguration = configuration
            .GetSection(rabbitMQReceiverConfigurationSection)
            .Get<RabbitMQReceiverConfiguration>();

        services.AddSingleton(rabbitMqReceiverConfiguration);
        
        var dbContextOptions = new DbContextOptionsBuilder<ReceiverContext>();
        dbContextOptions.UseNpgsql(configuration.GetConnectionString(connectionString));
        services.AddSingleton(new ReceiverContext(dbContextOptions.Options));
        
        services.AddSingleton<ISeatApplicationsDbRepository, SeatApplicationsDbRepository>();
        services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
        //services.AddSingleton<IReceiverService, ReceiverService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();