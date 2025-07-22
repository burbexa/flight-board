namespace FlightBoard.Domain;

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public DateTime DepartureTime { get; set; }
    public string Gate { get; set; } = null!;

}
