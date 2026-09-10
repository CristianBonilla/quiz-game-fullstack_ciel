using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Infrastructure.Idempotency;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");
        builder.HasKey(record => record.RequestId);
        builder.Property(record => record.RequestId).ValueGeneratedNever();

        builder.Property(record => record.CommandName).HasMaxLength(200).IsRequired();
        builder.Property(record => record.ResourceId);
        builder.Property(record => record.ResponsePayload).HasMaxLength(4000);
        builder.Property(record => record.StatusCode).IsRequired();
        builder.Property(record => record.CreatedOnUtc).IsRequired();
        builder.Property(record => record.ExpiresOnUtc).IsRequired();

        builder.HasIndex(record => record.RequestId)
            .IsUnique()
            .HasDatabaseName("UQ_IdempotencyRecords_RequestId");

        builder.HasIndex(record => record.ExpiresOnUtc).HasDatabaseName("IX_IdempotencyRecords_ExpiresOnUtc");
    }
}
