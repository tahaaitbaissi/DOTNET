using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.UserId)
                .IsRequired();

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Body)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(n => n.Type)
                .HasMaxLength(50);

            builder.Property(n => n.LastAttemptAt);

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.Status);
            builder.HasIndex(n => new { n.UserId, n.IsRead });

            // Relationships
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

