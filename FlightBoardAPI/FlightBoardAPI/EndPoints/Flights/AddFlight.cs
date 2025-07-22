using FlightBoard.API.Endpoints;
using FlightBoard.Application.Services;
using FlightBoard.Domain;

namespace FlightBoard.API.Endpoints.Flights;

public class AddFlight : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapPost("/api/flights", async (Flight flight, FlightService service) =>
        {
            try
            {
                await service.AddFlightAsync(flight);
                return Results.Created($"/api/flights/{flight.Id}", flight);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("AddFlight")
        .WithOpenApi();
    }
}
