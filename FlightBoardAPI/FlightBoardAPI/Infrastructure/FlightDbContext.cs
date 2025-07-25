﻿using FlightBoard.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlightBoard.Infrastructure;

public class FlightDbContext : DbContext
{
    public FlightDbContext(DbContextOptions<FlightDbContext> options)
        : base(options) { }

    public DbSet<Flight> Flights => Set<Flight>();
}
