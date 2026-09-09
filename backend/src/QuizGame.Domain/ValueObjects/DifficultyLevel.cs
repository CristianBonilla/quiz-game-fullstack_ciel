using QuizGame.Domain.SeedWork;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.ValueObjects;

public sealed class DifficultyLevel : ValueObject, IComparable<DifficultyLevel>
{
    public const int Minimum = 1;
    public const int Maximum = 5;

    private DifficultyLevel(int value) => Value = value;

    private DifficultyLevel()
    {
    }

    public int Value { get; }

    public static Result<DifficultyLevel> Create(int value) => value is < Minimum or > Maximum
        ? Failure<DifficultyLevel>(ValueObjectErrors.DifficultyLevelOutOfRange(value))
        : new DifficultyLevel(value);

    public int CompareTo(DifficultyLevel? other) => other is null ? 1 : Value.CompareTo(other.Value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override bool Equals(object? obj) => base.Equals(obj);

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(DifficultyLevel? left, DifficultyLevel? right) => Equals(left, right);

    public static bool operator !=(DifficultyLevel? left, DifficultyLevel? right) => !Equals(left, right);

    public static bool operator <(DifficultyLevel? left, DifficultyLevel? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(DifficultyLevel? left, DifficultyLevel? right) =>
        left is null || left.CompareTo(right) <= 0;

    public static bool operator >(DifficultyLevel? left, DifficultyLevel? right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(DifficultyLevel? left, DifficultyLevel? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
