using Domain.Shelfs;
using Domain.TradeOffers;
using Domain.TradeOfferEntrys;
using Domain.OwnershipHistorys;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context;

public class ShelfDbContext : DbContext
{
    public DbSet<Shelf> Shelfs => Set<Shelf>();
    public DbSet<TradeOffer> TradeOffers => Set<TradeOffer>();
    public DbSet<TradeOfferEntry> TradeOfferEntrys => Set<TradeOfferEntry>();
    public DbSet<OwnershipHistory> OwnershipHistorys => Set<OwnershipHistory>();

    public ShelfDbContext(DbContextOptions<ShelfDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShelfDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
