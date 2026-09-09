using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class RoundNumber : ValueObject, IComparable<RoundNumber>
{
    public const int First = 1;

    private RoundNumber(int value, int totalRounds)
    {
        Value = value;
        TotalRounds = totalRounds;
    }

    private RoundNumber()
    {
    }

    public int Value { get; }

    public int TotalRounds { get; }

    public bool IsFinal => Value == TotalRounds;

    public static Result<RoundNumber> Create(int value, int totalRounds) =>
        value < First || value > totalRounds
            ? Failure<RoundNumber>(ValueObjectErrors.RoundNumberOutOfRange(value, totalRounds))
            : new RoundNumber(value, totalRounds);

    public Result<RoundNumber> Next() => IsFinal
        ? Failure<RoundNumber>(ValueObjectErrors.RoundNumberIsFinal(Value))
        : new RoundNumber(Value + 1, TotalRounds);

    public int CompareTo(RoundNumber? other) => other is null ? 1 : Value.CompareTo(other.Value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return TotalRounds;
    }

    public override bool Equals(object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(RoundNumber? left, RoundNumber? right) => Equals(left, right);

    public static bool operator !=(RoundNumber? left, RoundNumber? right) => !Equals(left, right);

    public static bool operator <(RoundNumber? left, RoundNumber? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(RoundNumber? left, RoundNumber? right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(RoundNumber? left, RoundNumber? right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(RoundNumber? left, RoundNumber? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
