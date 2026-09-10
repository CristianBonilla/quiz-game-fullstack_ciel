using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Infrastructure.Idempotency;
using QuizGame.Infrastructure.Outbox;

namespace QuizGame.Infrastructure.Persistence;

public sealed class QuizGameDbContext(DbContextOptions<QuizGameDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<Game> Games => Set<Game>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuizGameDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
