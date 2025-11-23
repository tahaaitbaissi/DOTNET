using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.BookingId)
                .IsRequired();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.PaymentIntentId)
                .HasMaxLength(200);

            builder.Property(p => p.TransactionRef)
                .HasMaxLength(200);

            builder.Property(p => p.Notes)
                .HasMaxLength(1000);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(p => p.BookingId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.TransactionRef);

            // Relationships
            builder.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

