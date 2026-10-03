using Domain.Common;
using Domain.TradeOfferEntrys;
using Domain.TradeOffers;
using Domain.Shelfs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TradeOfferEntryConfiguration : IEntityTypeConfiguration<TradeOfferEntry>
{
    public void Configure(EntityTypeBuilder<TradeOfferEntry> builder)
    {
        builder.ToTable("TradeOfferEntrys");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired();

        builder.Property(u => u.TradeOfferId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("TradeOfferId");

        builder.Property(u => u.OwnerUserId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("OwnerUserId");

        builder.Property(u => u.ShelfId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("ShelfId");

        builder.HasOne<Shelf>()
            .WithMany()
            .HasForeignKey(u => u.ShelfId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TradeOfferEntry_Shelfs_ShelfId");

        builder.HasOne<TradeOffer>()
            .WithMany(t => t.Entries) 
            .HasForeignKey(u => u.TradeOfferId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_TradeOfferEntry_TradeOffers_TradeOfferId");
    }
}
