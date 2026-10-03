using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Context;

public class ShelfDbContextFactory : IDesignTimeDbContextFactory<ShelfDbContext>
{
    public ShelfDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                               ?? "Host=localhost;Database=ShelfDb;Shelfname=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ShelfDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ShelfDbContext(optionsBuilder.Options);
    }
}
