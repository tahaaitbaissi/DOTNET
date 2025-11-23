using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.ToTable("VehicleImages");

            builder.HasKey(vi => vi.Id);

            builder.Property(vi => vi.VehicleId)
                .IsRequired();

            builder.Property(vi => vi.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(vi => vi.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(vi => vi.MimeType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(vi => vi.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(vi => vi.CreatedAt)
                .IsRequired();

            builder.Property(vi => vi.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(vi => vi.VehicleId);
            builder.HasIndex(vi => new { vi.VehicleId, vi.IsPrimary });

            // Relationships
            builder.HasOne(vi => vi.Vehicle)
                .WithMany(v => v.Images)
                .HasForeignKey(vi => vi.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

