using FlightBoard.Application.Services;

namespace FlightBoard.API.Endpoints;

public class GetAllFlights : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/flights", async (FlightService flightService) =>
        {
            var flightsWithStatus = await flightService.GetAllWithStatusAsync();
            var result = flightsWithStatus.Select(x => new
            {
                x.flight.Id,
                x.flight.FlightNumber,
                x.flight.Destination,
                x.flight.DepartureTime,
                x.flight.Gate,
                Status = x.status.ToString()
            });

            return Results.Ok(result);
        })
        .WithName("GetAllFlights");
    }
}
