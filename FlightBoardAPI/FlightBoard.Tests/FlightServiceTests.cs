using FlightBoard.API.Hubs;
using FlightBoard.Application.Interfaces;
using FlightBoard.Application.Services;
using FlightBoard.Domain;
using FlightBoard.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class FlightServiceTests
{

    private readonly Mock<IFlightRepository> _mockRepo;
    private readonly Mock<IHubContext<FlightHub>> _mockHub;
    private readonly Mock<IClientProxy> _mockClientProxy;
    private readonly Mock<IHubClients> _mockClients;
    private readonly Mock<ILogger<FlightService>> _mockLogger;

    private readonly FlightService _service;

    public FlightServiceTests()
    {
        _mockRepo = new Mock<IFlightRepository>();
        _mockHub = new Mock<IHubContext<FlightHub>>();
        _mockClientProxy = new Mock<IClientProxy>();
        _mockClients = new Mock<IHubClients>();
        _mockLogger = new Mock<ILogger<FlightService>>();

        // Setup SignalR mock
        _mockClients.Setup(c => c.All).Returns(_mockClientProxy.Object);
        _mockHub.Setup(h => h.Clients).Returns(_mockClients.Object);

        _service = new FlightService(_mockRepo.Object, _mockHub.Object, _mockLogger.Object);
    }


    [Fact]
    public void CalculateStatus_ShouldReturnScheduled_WhenDepartureIsMoreThan30MinAway()
    {

        // Arrange
        var departureTime = DateTime.Now.AddMinutes(31);

        // Act
        var status = _service.CalculateStatus(departureTime);

        //Assert
        Assert.Equal(FlightStatus.Scheduled, status);
    }

    [Fact]
    public void CalculateStatus_ShouldReturnBoarding_WhenDepartureIsWithin30Min()
    {
        // Arrange
        var departureTime = DateTime.Now.AddMinutes(15);

        // Act
        var result = _service.CalculateStatus(departureTime);

        // Assert
        Assert.Equal(FlightStatus.Boarding, result);
    }

    [Fact]
    public void CalculateStatus_ShouldReturnDeparted_WhenDepartureWasLessThan60MinAgo()
    {
        // Arrange
        var departureTime = DateTime.Now.AddMinutes(-30);

        // Act
        var status = _service.CalculateStatus(departureTime);

        // Assert
        Assert.Equal(FlightStatus.Departed, status);
    }

    [Fact]
    public void CalculateStatus_ShouldReturnLanded_WhenDepartureWasMoreThan60MinAgo()
    {
        // Arrange
        var departureTime = DateTime.Now.AddMinutes(-90);

        // Act
        var status = _service.CalculateStatus(departureTime);

        // Assert
        Assert.Equal(FlightStatus.Landed, status);
    }



    
    [Fact]
    public async Task AddFlightAsync_ShouldThrow_WhenRequiredFieldsAreMissing()
    {
        // Arrange
        var invalidFlight = new Flight
        {
            FlightNumber = "  ",
            Destination = "",
            DepartureTime = DateTime.Now.AddHours(2),
            Gate = null
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddFlightAsync(invalidFlight)
        );

        Assert.Equal("All fields are required.", ex.Message);
    }


    [Fact]
    public async Task AddFlightAsync_ShouldThrow_WhenDepartureTimeIsInThePast()
    {
        // Arrange
        var invalidFlight = new Flight
        {
            FlightNumber = "ZZ999",
            Destination = "Atlantis",
            DepartureTime = DateTime.Now.AddMinutes(-1), // In the past
            Gate = "Z9"
        };


        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddFlightAsync(invalidFlight)
        );

        Assert.Equal("Departure time must be in the future.", ex.Message);
    }

    [Fact]
    public async Task AddFlightAsync_ShouldThrow_WhenFlightNumberExists()
    {
        // Arrange
        var existingFlightNumber = "AA123";
        var flight = new Flight
        {
            FlightNumber = existingFlightNumber,
            Destination = "Tokyo",
            DepartureTime = DateTime.Now.AddHours(1),
            Gate = "B4"
        };

        _mockRepo.Setup(r => r.ExistsByFlightNumberAsync(existingFlightNumber))
                 .ReturnsAsync(true); // Simulate duplicate

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddFlightAsync(flight));
    }

    [Fact]
    public async Task AddFlightAsync_ShouldSucceed_WhenValidFlight()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "ZZ999",
            Destination = "Berlin",
            DepartureTime = DateTime.Now.AddHours(2),
            Gate = "C2"
        };

        _mockRepo.Setup(r => r.ExistsByFlightNumberAsync(flight.FlightNumber))
                 .ReturnsAsync(false); // It's unique

        _mockRepo.Setup(r => r.AddAsync(flight))
                 .Returns(Task.CompletedTask); // Fake add

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _service.AddFlightAsync(flight));
        Assert.Null(exception);
    }


    [Fact]
    public async Task DeleteFlightAsync_ShouldSucceed_WhenFlightExists()
    {
        // Arrange
        var flightId = 1;
        var existingFlight = new Flight
        {
            Id = flightId,
            FlightNumber = "AB123",
            Destination = "Nowhere",
            DepartureTime = DateTime.Now.AddHours(2),
            Gate = "G1"
        };

        _mockRepo.Setup(r => r.GetByIdAsync(flightId)).ReturnsAsync(existingFlight);
        _mockRepo.Setup(r => r.DeleteAsync(flightId)).Returns(Task.CompletedTask);

        // Act
        var ex = await Record.ExceptionAsync(() => _service.DeleteFlightAsync(flightId));

        // Assert
        Assert.Null(ex);
        _mockRepo.Verify(r => r.DeleteAsync(flightId), Times.Once);
        _mockClients.Verify(c => c.All, Times.Once);
        _mockClientProxy.Verify(p => p.SendCoreAsync("FlightDeleted",
            It.Is<object[]>(o => (int)o[0] == flightId), default), Times.Once);
    }


    [Fact]
    public async Task DeleteFlightAsync_ShouldThrow_WhenFlightNotFound()
    {
        // Arrange
        var flightId = 99;
        
        _mockRepo.Setup(r => r.GetByIdAsync(flightId)).ReturnsAsync((Flight)null);

        // Act
        var ex = await Record.ExceptionAsync(() => _service.DeleteFlightAsync(flightId));

        // Assert
        Assert.NotNull(ex);
        Assert.IsType<ArgumentException>(ex);
        Assert.Equal("Flight not found.", ex.Message);

        _mockRepo.Verify(r => r.GetByIdAsync(flightId), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        _mockClients.Verify(c => c.All, Times.Never);
        _mockClientProxy.Verify(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default), Times.Never);
    }




    [Fact]
    public async Task GetAllWithStatusAsync_ShouldReturnCorrectStatuses()
    {
        // Arrange
        var flights = new List<Flight>
    {
        new() { Id = 1, FlightNumber = "A1", Destination = "Dest1", DepartureTime = DateTime.Now.AddMinutes(45) }, // Scheduled
        new() { Id = 2, FlightNumber = "A2", Destination = "Dest2", DepartureTime = DateTime.Now.AddMinutes(15) }, // Boarding
        new() { Id = 3, FlightNumber = "A3", Destination = "Dest3", DepartureTime = DateTime.Now.AddMinutes(-15) }, // Departed
        new() { Id = 4, FlightNumber = "A4", Destination = "Dest4", DepartureTime = DateTime.Now.AddMinutes(-90) }, // Landed
    };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(flights);

        // Act
        var result = await _service.GetAllWithStatusAsync();

        // Assert
        Assert.Collection(result,
            item => Assert.Equal(FlightStatus.Scheduled, item.status),
            item => Assert.Equal(FlightStatus.Boarding, item.status),
            item => Assert.Equal(FlightStatus.Departed, item.status),
            item => Assert.Equal(FlightStatus.Landed, item.status)
        );
    }


    [Fact]
    public async Task GetAllWithStatusAsync_ShouldReturnEmptyList_WhenNoFlightsExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Flight>());

        // Act
        var result = await _service.GetAllWithStatusAsync();

        // Assert
        Assert.NotNull(result); // Should return an empty list, not null
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetAllWithStatusAsync_ShouldThrow_WhenRepositoryFails()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB down"));

        // Act
        var ex = await Record.ExceptionAsync(() => _service.GetAllWithStatusAsync());

        // Assert
        Assert.NotNull(ex);
        Assert.IsType<Exception>(ex);
        Assert.Equal("DB down", ex.Message);
    }
}
