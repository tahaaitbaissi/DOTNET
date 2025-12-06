using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.VIN)
                .IsRequired()
                .HasMaxLength(17);

            builder.Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Year)
                .IsRequired();

            builder.Property(v => v.Color)
                .HasMaxLength(50);

            builder.Property(v => v.LicensePlate)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(v => v.IsInsured)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(v => v.InsurancePolicy)
                .HasMaxLength(100);

            builder.Property(v => v.Geometry)
                .IsRequired();

            builder.Property(v => v.Issues)
                .HasMaxLength(1000);

            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(v => v.VIN)
                .IsUnique();

            builder.HasIndex(v => v.LicensePlate)
                .IsUnique();

            builder.HasIndex(v => v.Status);

            // VehicleType relationship
            builder.HasOne(v => v.VehicleType)
                .WithMany(vt => vt.Vehicles)
                .HasForeignKey(v => v.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationships
            builder.HasMany(v => v.Bookings)
                .WithOne(b => b.Vehicle)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Images)
                .WithOne(vi => vi.Vehicle)
                .HasForeignKey(vi => vi.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.MaintenanceHistory)
                .WithOne(m => m.Vehicle)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Documents)
                .WithOne(d => d.Vehicle)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Tariffs)
                .WithOne(t => t.Vehicle)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

