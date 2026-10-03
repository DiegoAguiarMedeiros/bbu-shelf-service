using Domain.Common;
using Domain.Common.Enums;
using Domain.Shelfs;
using Domain.Shelfs.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        builder.ToTable("Shelfs");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired();

        builder.Property(u => u.UserId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("UserId");
            
        builder.Property(u => u.BookId)
            .HasConversion(
                id => id.Value,
                value => Id.From(value)
            )
            .IsRequired()
        .HasColumnName("BookId");

        builder.Property(u => u.AcquiredSource)
            .HasConversion(
                acquiredSource => acquiredSource.ToString(),
                value => Enum.Parse<AcquiredSource>(value)
            )
            .IsRequired();

        builder.Property(u => u.CopyType)
            .HasConversion(
                copyType => copyType.ToString(),
                value => Enum.Parse<CopyType>(value)
            )
            .IsRequired();

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt);
    }
}
