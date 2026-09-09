using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class AnswerText : ValueObject
{
    public const int MaxLength = 300;

    private AnswerText(string value) => Value = value;

    private AnswerText() => Value = string.Empty;

    public string Value { get; } = string.Empty;

    public static Result<AnswerText> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Failure<AnswerText>(ValueObjectErrors.EmptyAnswerText);
        }

        string trimmed = value.Trim();

        return trimmed.Length > MaxLength
            ? Failure<AnswerText>(ValueObjectErrors.AnswerTextTooLong(MaxLength))
            : new AnswerText(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
