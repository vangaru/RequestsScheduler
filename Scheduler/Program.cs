using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Services;
using RequestsScheduler.RabbitMQClient;
using RequestsScheduler.Scheduler;
using RequestsScheduler.Scheduler.Services;

const string intervalsConfigurationSection = "IntervalsConfiguration";
const string routesConfigurationSection= "RoutesConfiguration";
const string rabbitMQConfigurationSection = "RabbitMQConfiguration";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        
        var intervalsConfiguration = configuration
            .GetSection(intervalsConfigurationSection)
            .Get<IntervalsConfiguration>();

        var routesConfiguration = configuration
            .GetSection(routesConfigurationSection)
            .Get<RoutesConfiguration>();

        var rabbitMqConfiguration = configuration
            .GetSection(rabbitMQConfigurationSection)
            .Get<RabbitMQConfiguration>();

        services.AddSingleton(intervalsConfiguration);
        services.AddSingleton(routesConfiguration);
        services.AddSingleton(rabbitMqConfiguration);
        services.AddSingleton<IIntervalsProvider, IntervalsProvider>();
        services.AddSingleton<INextSeatApplicationDelayProvider, NextSeatApplicationDelayProvider>();
        services.AddSingleton<IRabbitMQRepository, RabbitMQRepository>();
        services.AddSingleton<IRouteGenerator, RouteGenerator>();
        services.AddSingleton<ISeatApplicationGenerator, SeatApplicationGenerator>();
        services.AddSingleton<ISchedulerService, SchedulerService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();