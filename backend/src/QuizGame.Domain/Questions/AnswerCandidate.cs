using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.Questions;

public sealed record AnswerCandidate(Guid Id, AnswerText Text, bool IsCorrect);
