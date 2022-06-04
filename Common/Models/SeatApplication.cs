using System.Text.Json;

namespace RequestsScheduler.Common.Models;

public class SeatApplication
{
    public string? Id { get; set; }
    public int SeatsCount { get; set; }
    public int Origin { get; set; }
    public int Destination { get; set; }
    public DateTime DateTime { get; set; }
    public string? Status { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}