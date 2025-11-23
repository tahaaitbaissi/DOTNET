using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.KeyValues)
                .HasMaxLength(500);

            builder.Property(a => a.Entity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.PerformedBy)
                .HasMaxLength(100);

            builder.Property(a => a.TransactionId)
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(a => a.Entity);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.TransactionId);
        }
    }
}

