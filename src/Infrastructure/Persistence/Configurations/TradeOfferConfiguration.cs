using Domain.Common;
using Domain.TradeOffers;
using Domain.TradeOffers.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TradeOfferConfiguration : IEntityTypeConfiguration<TradeOffer>
{
    public void Configure(EntityTypeBuilder<TradeOffer> builder)
    {
        builder.ToTable("TradeOffers");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired();

        builder.Property(u => u.FromUserId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("FromUserId");

        builder.Property(u => u.ToUserId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("ToUserId");
            
        builder.Property(u => u.Status)
            .HasConversion(
                status => status.ToString(),
                value => Enum.Parse<Status>(value)
            )
            .IsRequired();
        
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt);

        
    }
}
