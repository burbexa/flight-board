using FlightBoard.API.Endpoints;
using FlightBoard.API.Hubs;
using FlightBoard.API.Services;
using FlightBoard.Application.Interfaces;
using FlightBoard.Application.Services;
using FlightBoard.Infrastructure;
using FlightBoard.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Host.UseSerilog(); 


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FlightDbContext>(options =>
    options.UseSqlite("Data Source=flights.db"));

builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<FlightService>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // No trailing slash!
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for SignalR/WebSockets
    });
});

builder.Services.AddHostedService<FlightStatusBackgroundService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.RegisterEndpoints();
    
}

app.UseCors("AllowFrontend");
app.MapHub<FlightHub>("/flightHub");
app.Run();
