using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.ToTable("VehicleTypes");

            builder.HasKey(vt => vt.Id);

            builder.Property(vt => vt.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(vt => vt.Description)
                .HasMaxLength(500);

            builder.Property(vt => vt.BaseRate)
                .IsRequired()
                .HasPrecision(18, 2);

            // Relationships
            builder.HasMany(vt => vt.Vehicles)
                .WithOne(v => v.VehicleType)
                .HasForeignKey(v => v.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(vt => vt.Tariffs)
                .WithOne(t => t.VehicleType)
                .HasForeignKey(t => t.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
