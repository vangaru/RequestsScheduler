using RequestsScheduler.Common.Configuration;

namespace RequestsScheduler.Common.Services;

public class RouteGenerator : IRouteGenerator
{
    private readonly RoutesConfiguration _routesConfiguration;

    public RouteGenerator(RoutesConfiguration routesConfiguration)
    {
        _routesConfiguration = routesConfiguration;
    }

    public (int origin, int destination) Generate()
    {
        if (_routesConfiguration.RoutesCount <= 0)
        {
            throw new ApplicationException("Number of routes must be larger than zero.");
        }
        
        var random = new Random();
        int origin = random.Next(0, _routesConfiguration.RoutesCount);
        int destination = random.Next(0, _routesConfiguration.RoutesCount);
        if (origin == destination)
        {
            Generate();
        }

        return (origin, destination);
    }
}