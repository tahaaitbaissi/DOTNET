using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.BookingId);

            builder.Property(d => d.VehicleId);

            builder.Property(d => d.ClientId);

            builder.Property(d => d.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.DocumentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.UploadedBy);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(d => d.BookingId);
            builder.HasIndex(d => d.VehicleId);
            builder.HasIndex(d => d.ClientId);
            builder.HasIndex(d => d.DocumentType);

            // Relationships
            builder.HasOne(d => d.Booking)
                .WithMany(b => b.Documents)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Vehicle)
                .WithMany(v => v.Documents)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Client)
                .WithMany(c => c.Documents)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

