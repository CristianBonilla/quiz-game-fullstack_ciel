using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.TestUtilities;

public sealed class CategoryBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Sample Category";
    private string _description = "Sample description";
    private int _difficultyLevel = 1;
    private decimal _prizeAmount = 100m;
    private int _questionCount;
    private bool _activated;
    private bool _deactivated;

    public static CategoryBuilder ACategory() => new();

    public CategoryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithDifficultyLevel(int level)
    {
        _difficultyLevel = level;
        return this;
    }

    public CategoryBuilder WithPrizeAmount(decimal amount)
    {
        _prizeAmount = amount;
        return this;
    }

    public CategoryBuilder WithQuestions(int count)
    {
        _questionCount = count;
        return this;
    }

    public CategoryBuilder Activated()
    {
        _activated = true;
        _deactivated = false;
        return this;
    }

    public CategoryBuilder Deactivated()
    {
        _deactivated = true;
        _activated = false;
        return this;
    }

    public Category Build()
    {
        Category category = Category.Create(
            _id,
            _name,
            _description,
            DifficultyLevel.Create(_difficultyLevel).Value,
            Prize.Create(_prizeAmount).Value).Value;

        for (int index = 0; index < _questionCount; index++)
        {
            Question question = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();
            category.AddQuestion(question);
        }

        if (_deactivated)
        {
            category.Deactivate();
        }
        else if (_activated && !category.IsActive)
        {
            category.Activate();
        }

        return category;
    }
}
