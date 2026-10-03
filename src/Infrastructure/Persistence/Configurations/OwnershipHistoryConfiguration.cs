using Domain.Common;
using Domain.Common.Enums;
using Domain.OwnershipHistorys;
using Domain.TradeOffers;
using Domain.Shelfs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OwnershipHistoryConfiguration : IEntityTypeConfiguration<OwnershipHistory>
{
    public void Configure(EntityTypeBuilder<OwnershipHistory> builder)
    {
        builder.ToTable("OwnershipHistorys");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired();

        builder.Property(u => u.ShelfId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
            .HasColumnName("ShelfId");

        // --- FromUserId: NULLABLE (null na primeira aquisição/compra) ---
        builder.Property(u => u.FromUserId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : Id.From(value.Value)
            )
            .IsRequired(false)
            .HasColumnName("FromUserId");

        builder.Property(u => u.ToUserId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
            .HasColumnName("ToUserId");

        // --- TradeOfferId: NULLABLE (só preenchido quando source = trade) ---
        builder.Property(u => u.TradeOfferId)
            .HasConversion(
                id => id == null ? (Guid?)null : id.Value,
                value => value == null ? null : Id.From(value.Value)
            )
            .IsRequired(false)
            .HasColumnName("TradeOfferId");

        builder.Property(o => o.TransferredAt)
            .IsRequired();

        builder.Property(u => u.AcquiredSource)
            .HasConversion(
                acquiredSource => acquiredSource.ToString(),
                value => Enum.Parse<AcquiredSource>(value)
            )
            .IsRequired();

        // --- Relacionamento com Shelf ---
        builder.HasOne<Shelf>()
            .WithMany()
            .HasForeignKey(o => o.ShelfId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_OwnershipHistory_Shelfs_ShelfId");

        // --- Relacionamento com TradeOffer (nullable) ---
        builder.HasOne<TradeOffer>()
            .WithMany()
            .HasForeignKey(o => o.TradeOfferId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OwnershipHistory_TradeOffers_TradeOfferId");

        // --- Índices ---
        builder.HasIndex(o => new { o.ShelfId, o.TransferredAt })
            .HasDatabaseName("IX_OwnershipHistory_ShelfId_TransferredAt");

        builder.HasIndex(o => o.TradeOfferId)
            .HasDatabaseName("IX_OwnershipHistory_TradeOfferId");
    }
}