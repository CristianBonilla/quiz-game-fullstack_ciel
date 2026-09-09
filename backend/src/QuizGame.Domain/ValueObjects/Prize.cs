using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class Prize : ValueObject, IComparable<Prize>
{
    public static readonly Prize Zero = new(0m);

    private Prize(decimal amount) => Amount = amount;

    private Prize()
    {
    }

    public decimal Amount { get; }

    public static Result<Prize> Create(decimal amount) => amount < 0m
        ? Failure<Prize>(ValueObjectErrors.NegativePrize)
        : new Prize(amount);

    public Prize Add(Prize other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return new Prize(Amount + other.Amount);
    }

    public int CompareTo(Prize? other) => other is null ? 1 : Amount.CompareTo(other.Amount);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
    }

    public override bool Equals(object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(Prize? left, Prize? right) => Equals(left, right);

    public static bool operator !=(Prize? left, Prize? right) => !Equals(left, right);

    public static bool operator <(Prize? left, Prize? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Prize? left, Prize? right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Prize? left, Prize? right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Prize? left, Prize? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
