using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Strategies;

public sealed record QuestionSelectionContext(
    Guid CategoryId,
    IReadOnlyCollection<Guid> AskedQuestionIds,
    RoundNumber RoundNumber);
