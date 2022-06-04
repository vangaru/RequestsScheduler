using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Services;
using RequestsScheduler.Scheduler;
using RequestsScheduler.Scheduler.Services;

const string intervalsConfigurationSection = "IntervalsConfiguration";

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        IntervalsConfiguration intervalsConfiguration = configuration
            .GetSection(intervalsConfigurationSection)
            .Get<IntervalsConfiguration>();

        services.AddSingleton(intervalsConfiguration);
        services.AddSingleton<IIntervalsProvider, IntervalsProvider>();
        services.AddSingleton<INextSeatApplicationDelayProvider, NextSeatApplicationDelayProvider>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();