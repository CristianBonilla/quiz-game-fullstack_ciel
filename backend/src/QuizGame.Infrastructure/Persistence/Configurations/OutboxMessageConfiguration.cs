using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Infrastructure.Outbox;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).ValueGeneratedNever();

        builder.Property(message => message.Type).HasMaxLength(400).IsRequired();
        builder.Property(message => message.Content).HasMaxLength(4000).IsRequired();
        builder.Property(message => message.OccurredOnUtc).IsRequired();
        builder.Property(message => message.ProcessedOnUtc);
        builder.Property(message => message.AttemptCount).IsRequired();
        builder.Property(message => message.Error).HasMaxLength(2000);

        builder.HasIndex(message => message.ProcessedOnUtc)
            .HasFilter("[ProcessedOnUtc] IS NULL")
            .HasDatabaseName("IX_OutboxMessages_Pending");
    }
}
