using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    public const int MinimumQuestions = 5;
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 400;

    private readonly List<Question> _questions = [];

    private Category(
        Guid id,
        string name,
        string description,
        DifficultyLevel difficultyLevel,
        Prize prizeAmount) : base(id)
    {
        Name = name;
        Description = description;
        DifficultyLevel = difficultyLevel;
        PrizeAmount = prizeAmount;
        IsActive = false;
    }

    private Category()
    {
        Name = string.Empty;
        Description = string.Empty;
        DifficultyLevel = null!;
        PrizeAmount = null!;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DifficultyLevel DifficultyLevel { get; private set; }

    public Prize PrizeAmount { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    public static Result<Category> Create(
        Guid id,
        string? name,
        string? description,
        DifficultyLevel difficultyLevel,
        Prize prizeAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Failure<Category>(CategoryErrors.EmptyName);
        }

        string trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Failure<Category>(CategoryErrors.NameTooLong);
        }

        string trimmedDescription = description?.Trim() ?? string.Empty;

        return trimmedDescription.Length > DescriptionMaxLength
            ? Failure<Category>(CategoryErrors.DescriptionTooLong)
            : new Category(id, trimmedName, trimmedDescription, difficultyLevel, prizeAmount);
    }

    public Result Update(
        string? name,
        string? description,
        DifficultyLevel difficultyLevel,
        Prize prizeAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CategoryErrors.EmptyName);
        }

        string trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure(CategoryErrors.NameTooLong);
        }

        string trimmedDescription = description?.Trim() ?? string.Empty;
        if (trimmedDescription.Length > DescriptionMaxLength)
        {
            return Result.Failure(CategoryErrors.DescriptionTooLong);
        }

        Name = trimmedName;
        Description = trimmedDescription;
        DifficultyLevel = difficultyLevel;
        PrizeAmount = prizeAmount;

        return Success();
    }

    public Result AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        if (question.CategoryId != Id)
        {
            return Result.Failure(CategoryErrors.QuestionFromAnotherCategory);
        }

        if (_questions.Exists(existing => existing.Id == question.Id))
        {
            return Result.Failure(CategoryErrors.DuplicateQuestion);
        }

        _questions.Add(question);

        return Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(CategoryErrors.AlreadyActive);
        }

        if (_questions.Count(question => question.IsActive) < MinimumQuestions)
        {
            return Result.Failure(CategoryErrors.NotEnoughQuestions);
        }

        IsActive = true;

        return Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(CategoryErrors.AlreadyInactive);
        }

        IsActive = false;

        return Success();
    }
}
