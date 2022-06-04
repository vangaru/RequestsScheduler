using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Services;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Scheduler;
using RequestsScheduler.Scheduler.Services;

const string intervalsConfigurationSection = "IntervalsConfiguration";
const string rabbitMQConfigurationSection = "RabbitMQConfiguration";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        
        IntervalsConfiguration intervalsConfiguration = configuration
            .GetSection(intervalsConfigurationSection)
            .Get<IntervalsConfiguration>();

        RabbitMQConfiguration rabbitMqConfiguration = configuration
            .GetSection(rabbitMQConfigurationSection)
            .Get<RabbitMQConfiguration>();

        services.AddSingleton(intervalsConfiguration);
        services.AddSingleton(rabbitMqConfiguration);
        services.AddSingleton<IIntervalsProvider, IntervalsProvider>();
        services.AddSingleton<INextSeatApplicationDelayProvider, NextSeatApplicationDelayProvider>();
        services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();