using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.Games;

public sealed class Round : Entity<Guid>
{
    private Round(
        Guid id,
        Guid gameId,
        RoundNumber number,
        Guid questionId,
        Guid correctAnswerId,
        Prize prizeAtStake,
        DateTime deadlineUtc) : base(id)
    {
        GameId = gameId;
        Number = number;
        QuestionId = questionId;
        CorrectAnswerId = correctAnswerId;
        PrizeAtStake = prizeAtStake;
        DeadlineUtc = deadlineUtc;
        Outcome = RoundOutcome.Pending;
    }

    private Round()
    {
        Number = null!;
        PrizeAtStake = null!;
    }

    public Guid GameId { get; private set; }

    public RoundNumber Number { get; private set; }

    public Guid QuestionId { get; private set; }

    // Snapshot taken when the round opens so the game can score itself without reloading the question.
    public Guid CorrectAnswerId { get; private set; }

    public Prize PrizeAtStake { get; private set; }

    public Guid? SelectedAnswerId { get; private set; }

    public RoundOutcome Outcome { get; private set; }

    public DateTime DeadlineUtc { get; private set; }

    public DateTime? AnsweredAtUtc { get; private set; }

    public bool IsAnswered => Outcome != RoundOutcome.Pending;

    internal static Round Open(
        Guid id,
        Guid gameId,
        RoundNumber number,
        Guid questionId,
        Guid correctAnswerId,
        Prize prizeAtStake,
        DateTime deadlineUtc) =>
        new(id, gameId, number, questionId, correctAnswerId, prizeAtStake, deadlineUtc);

    internal void RegisterAnswer(Guid selectedAnswerId, RoundOutcome outcome, DateTime answeredAtUtc)
    {
        SelectedAnswerId = selectedAnswerId;
        Outcome = outcome;
        AnsweredAtUtc = answeredAtUtc;
    }
}
