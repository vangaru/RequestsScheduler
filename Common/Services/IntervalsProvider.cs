using System.Text.Json;
using RequestsScheduler.Common.Configuration;
using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Common.Services;

public class IntervalsProvider : IIntervalsProvider
{
    private readonly IntervalsConfiguration _intervalsConfiguration;

    public IntervalsProvider(IntervalsConfiguration intervalsConfiguration)
    {
        _intervalsConfiguration = intervalsConfiguration;
        Intervals = GetIntervalsFromConfig();
    }

    private IEnumerable<Interval> GetIntervalsFromConfig()
    {
        string? intervalsConfigPath = _intervalsConfiguration.IntervalsConfigurationFilePath;
        if (intervalsConfigPath == null)
        {
            throw new ApplicationException(
                "Intervals configuration file path cannot be null. Please, define it's path in appsettings.json.");
        }

        if (!File.Exists(intervalsConfigPath))
        {
            throw new FileNotFoundException("Cannot find path to the intervals configuration file.", 
                intervalsConfigPath);
        }

        string intervalsConfigurationContent = File.ReadAllText(intervalsConfigPath);
        var intervals = JsonSerializer.Deserialize<IEnumerable<Interval>>(intervalsConfigurationContent);

        if (intervals == null)
        {
            throw new ApplicationException($"Failed to deserialize contents of {intervalsConfigPath}");
        }

        return intervals;
    }

    public Interval? CurrentInterval
    {
        get
        {
            if (Intervals == null)
            {
                throw new ApplicationException("Cannot get current interval. Intervals are null.");
            }

            return Intervals.FirstOrDefault(interval => interval.Hour == DateTime.Now.Hour);
        }
    }

    public IEnumerable<Interval>? Intervals { get; }
}