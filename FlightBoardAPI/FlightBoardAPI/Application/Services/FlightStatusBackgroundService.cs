// FlightStatusBackgroundService.cs
using Microsoft.AspNetCore.SignalR;

using FlightBoard.API.Hubs;

namespace FlightBoard.API.Services
{
    public class FlightStatusBackgroundService : BackgroundService
    {
        private readonly IHubContext<FlightHub> _hubContext;
        private readonly ILogger<FlightStatusBackgroundService> _logger;

        public FlightStatusBackgroundService(
            IHubContext<FlightHub> hubContext,
            ILogger<FlightStatusBackgroundService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("[FlightStatusBackgroundService] Broadcasting status update...");
                    await _hubContext.Clients.All.SendAsync("FlightsUpdated", cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error broadcasting FlightsUpdated");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
