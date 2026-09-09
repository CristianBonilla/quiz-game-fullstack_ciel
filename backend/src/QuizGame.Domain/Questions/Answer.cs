using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.Questions;

public sealed class Answer : Entity<Guid>
{
    private Answer(Guid id, Guid questionId, AnswerText text, bool isCorrect) : base(id)
    {
        QuestionId = questionId;
        Text = text;
        IsCorrect = isCorrect;
    }

    private Answer() => Text = null!;

    public Guid QuestionId { get; private set; }

    public AnswerText Text { get; private set; }

    public bool IsCorrect { get; private set; }

    internal static Answer Create(Guid id, Guid questionId, AnswerText text, bool isCorrect) =>
        new(id, questionId, text, isCorrect);
}
