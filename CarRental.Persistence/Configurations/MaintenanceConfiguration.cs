using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.ToTable("MaintenanceRecords");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.VehicleId)
                .IsRequired();

            builder.Property(m => m.StartDate)
                .IsRequired();

            builder.Property(m => m.EndDate);

            builder.Property(m => m.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Details)
                .HasMaxLength(2000);

            builder.Property(m => m.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.Cost)
                .HasPrecision(18, 2);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(m => m.VehicleId);
            builder.HasIndex(m => m.IsCompleted);
            builder.HasIndex(m => new { m.VehicleId, m.IsCompleted });

            // Relationships
            builder.HasOne(m => m.Vehicle)
                .WithMany(v => v.MaintenanceHistory)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

