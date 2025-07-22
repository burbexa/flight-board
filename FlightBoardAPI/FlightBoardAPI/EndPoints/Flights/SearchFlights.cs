using FlightBoard.API.Endpoints;
using FlightBoard.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightBoard.API.Endpoints.Flights;

public class SearchFlights : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGet("/api/flights/search", async (
            [FromQuery] string? status,
            [FromQuery] string? destination,
            FlightService service) =>
        {
            var flightsWithStatus = await service.GetAllWithStatusAsync();

            var filtered = flightsWithStatus
                .Where(f =>
                    (string.IsNullOrWhiteSpace(status) || f.status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(destination) || f.flight.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase)))
                .Select(x => new
                {
                    x.flight.Id,
                    x.flight.FlightNumber,
                    x.flight.Destination,
                    x.flight.DepartureTime,
                    x.flight.Gate,
                    Status = x.status.ToString()
                });

            return Results.Ok(filtered);
        })
        .WithName("SearchFlights")
        .WithOpenApi();
    }
}
