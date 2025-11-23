using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
    {
        public void Configure(EntityTypeBuilder<Tariff> builder)
        {
            builder.ToTable("Tariffs");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.VehicleTypeId);

            builder.Property(t => t.VehicleId);

            builder.Property(t => t.PricePerHour)
                .HasPrecision(18, 2);

            builder.Property(t => t.PricePerDay)
                .HasPrecision(18, 2);

            builder.Property(t => t.PricePerKm)
                .HasPrecision(18, 2);

            builder.Property(t => t.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("USD");

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.EffectiveTo);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(t => t.VehicleTypeId);
            builder.HasIndex(t => t.VehicleId);
            builder.HasIndex(t => new { t.IsActive, t.StartDate, t.EffectiveTo });

            // Relationships
            builder.HasOne(t => t.VehicleType)
                .WithMany(vt => vt.Tariffs)
                .HasForeignKey(t => t.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Vehicle)
                .WithMany(v => v.Tariffs)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

