using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.DriverLicense)
                .HasMaxLength(50);

            builder.Property(c => c.LicenseExpiry)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(c => c.UserId)
                .IsUnique();

            builder.HasIndex(c => c.DriverLicense);

            // Relationships
            builder.HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Bookings)
                .WithOne(b => b.Client)
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Documents)
                .WithOne(d => d.Client)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

