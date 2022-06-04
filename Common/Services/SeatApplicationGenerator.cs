using RequestsScheduler.Common.Models;

namespace RequestsScheduler.Common.Services;

public class SeatApplicationGenerator : ISeatApplicationGenerator
{
    private const int MaxSeatsCount = 3;
    
    private readonly IRouteGenerator _routeGenerator;

    public SeatApplicationGenerator(IRouteGenerator routeGenerator)
    {
        _routeGenerator = routeGenerator;
    }

    public SeatApplication Generate()
    {
        (int origin, int destination) = _routeGenerator.Generate();

        return new SeatApplication
        {
            Id = Guid.NewGuid().ToString(),
            DateTime = DateTime.Now,
            Origin = origin,
            Destination = destination,
            SeatsCount = GenerateSeatsCount(),
        };
    }

    private int GenerateSeatsCount()
    {
        var random = new Random();
        return random.Next(0, MaxSeatsCount);
    }
}