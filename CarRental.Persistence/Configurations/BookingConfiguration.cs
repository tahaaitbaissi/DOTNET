using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.ClientId)
                .IsRequired();

            builder.Property(b => b.VehicleId)
                .IsRequired();

            builder.Property(b => b.CreatedBy);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(b => b.StartDate)
                .IsRequired();

            builder.Property(b => b.EndDate)
                .IsRequired();

            builder.Property(b => b.PickUpLocation)
                .HasMaxLength(500);

            builder.Property(b => b.DropOffLocation)
                .HasMaxLength(500);

            builder.Property(b => b.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(b => b.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.Notes)
                .HasMaxLength(2000);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(b => b.ClientId);
            builder.HasIndex(b => b.VehicleId);
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.StartDate);
            builder.HasIndex(b => new { b.VehicleId, b.StartDate, b.EndDate, b.Status });

            // Relationships
            builder.HasOne(b => b.Client)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Vehicle)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Documents)
                .WithOne(d => d.Booking)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

