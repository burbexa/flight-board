using FlightBoard.Domain;

namespace FlightBoard.Application.Interfaces;

public interface IFlightRepository
{
    Task<List<Flight>> GetAllAsync();
    Task<Flight?> GetByIdAsync(int id);
    Task<List<Flight>> SearchAsync(string? status, string? destination);
    Task AddAsync(Flight flight);
    Task DeleteAsync(int id);
    Task<bool> ExistsByFlightNumberAsync(string flightNumber);
}
