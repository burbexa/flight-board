using FlightBoard.API.Endpoints;
using FlightBoard.Application.Services;

namespace FlightBoard.API.Endpoints.Flights;

public class DeleteFlight : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapDelete("/api/flights/{id:int}", async (int id, FlightService service) =>
        {
            await service.DeleteFlightAsync(id);
            return Results.Ok();
        })
        .WithName("DeleteFlight")
        .WithOpenApi();
    }
}
