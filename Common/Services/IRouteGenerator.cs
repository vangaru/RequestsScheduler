namespace RequestsScheduler.Common.Services;

public interface IRouteGenerator
{
    public (int origin, int destination) Generate();
}