using FlightBoard.Domain;
using FlightBoard.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlightBoard.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly FlightDbContext _context;

    public FlightRepository(FlightDbContext context)
    {
        _context = context;
    }

    public async Task<List<Flight>> GetAllAsync() =>
        await _context.Flights.ToListAsync();

    public async Task<Flight?> GetByIdAsync(int id) =>
        await _context.Flights.FindAsync(id);

    public async Task<List<Flight>> SearchAsync(string? status, string? destination)
    {
        var query = _context.Flights.AsQueryable();

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(f => f.Destination.Contains(destination));

        return await query.ToListAsync();
    }

    public async Task AddAsync(Flight flight)
    {
        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flight = await _context.Flights.FindAsync(id);
        if (flight != null)
        {
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByFlightNumberAsync(string flightNumber) =>
        await _context.Flights.AnyAsync(f => f.FlightNumber == flightNumber);
}
