using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class PlayerName : ValueObject
{
    public const int MaxLength = 60;

    private PlayerName(string value) => Value = value;

    private PlayerName() => Value = string.Empty;

    public string Value { get; } = string.Empty;

    public static Result<PlayerName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Failure<PlayerName>(ValueObjectErrors.EmptyPlayerName);
        }

        string trimmed = value.Trim();

        return trimmed.Length > MaxLength
            ? Failure<PlayerName>(ValueObjectErrors.PlayerNameTooLong(MaxLength))
            : new PlayerName(trimmed);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
