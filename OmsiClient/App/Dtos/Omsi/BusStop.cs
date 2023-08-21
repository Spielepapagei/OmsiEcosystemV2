namespace OmsiDataClient.Models;

public class BusStop
{
    public ulong StopId { get; set; }
    public int InGameId { get; set; }
    public string Name { get; set; } = "Unknown";
}