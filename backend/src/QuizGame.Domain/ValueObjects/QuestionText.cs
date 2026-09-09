using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class QuestionText : ValueObject
{
    public const int MaxLength = 500;

    private QuestionText(string value) => Value = value;

    private QuestionText() => Value = string.Empty;

    public string Value { get; } = string.Empty;

    public static Result<QuestionText> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Failure<QuestionText>(ValueObjectErrors.EmptyQuestionText);
        }

        string trimmed = value.Trim();

        return trimmed.Length > MaxLength
            ? Failure<QuestionText>(ValueObjectErrors.QuestionTextTooLong(MaxLength))
            : new QuestionText(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
