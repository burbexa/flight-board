using FlightBoard.Application.Interfaces;
using FlightBoard.Domain;
using Microsoft.AspNetCore.SignalR;
using FlightBoard.API.Hubs;


namespace FlightBoard.Application.Services;


public class FlightService
{
    private readonly IHubContext<FlightHub> _hub;
    private readonly IFlightRepository _repo;
    private readonly ILogger<FlightService> _logger;

    public FlightService(IFlightRepository repo, IHubContext<FlightHub> hub, ILogger<FlightService> logger)
    {
        _repo = repo;
        _hub = hub;
        _logger = logger;
    }

    public async Task<List<(Flight flight, FlightStatus status)>> GetAllWithStatusAsync()
    {
        var flights = await _repo.GetAllAsync();
        return flights
            .Select(f => (f, CalculateStatus(f.DepartureTime)))
            .ToList();
    }

    public async Task AddFlightAsync(Flight flight)
    {
        _logger.LogInformation("Adding flight {FlightNumber} to {Destination}", flight.FlightNumber, flight.Destination);

        if (string.IsNullOrWhiteSpace(flight.FlightNumber) ||
            string.IsNullOrWhiteSpace(flight.Destination) ||
            string.IsNullOrWhiteSpace(flight.Gate))
            throw new ArgumentException("All fields are required.");

        if (flight.DepartureTime <= DateTime.Now)
            throw new ArgumentException("Departure time must be in the future.");

        if (await _repo.ExistsByFlightNumberAsync(flight.FlightNumber))
            throw new ArgumentException("Flight number must be unique.");

        await _repo.AddAsync(flight);

        // Broadcast only AFTER the DB is fully updated
        await _hub.Clients.All.SendAsync("FlightAdded", flight);

        _logger.LogInformation("Flight {FlightNumber} successfully added", flight.FlightNumber);
    }

    public async Task DeleteFlightAsync(int id)
    {
        _logger.LogInformation("Deleting flight with ID {FlightId}", id);

        var flight = await _repo.GetByIdAsync(id);
        if (flight == null)
            throw new ArgumentException("Flight not found.");

        await _repo.DeleteAsync(id);
        await _hub.Clients.All.SendAsync("FlightDeleted", id);

        _logger.LogInformation("Flight {FlightId} deleted", id);
    }


    public FlightStatus CalculateStatus(DateTime departure)
    {
        var now = DateTime.Now;
        var diff = departure - now;

        if (diff.TotalMinutes > 30) return FlightStatus.Scheduled;
        if (diff.TotalMinutes <= 30 && diff.TotalMinutes >= 0) return FlightStatus.Boarding;
        if (diff.TotalMinutes < 0 && diff.TotalMinutes >= -60) return FlightStatus.Departed;
        return FlightStatus.Landed;
    }
}
