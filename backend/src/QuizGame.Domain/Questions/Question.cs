using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.Questions;

public sealed class Question : AggregateRoot<Guid>
{
    public const int RequiredAnswers = 4;

    private readonly List<Answer> _answers = [];

    private Question(Guid id, Guid categoryId, QuestionText text) : base(id)
    {
        CategoryId = categoryId;
        Text = text;
        IsActive = true;
    }

    private Question() => Text = null!;

    public Guid CategoryId { get; private set; }

    public QuestionText Text { get; private set; }

    public bool IsActive { get; private set; }

    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

    public Guid CorrectAnswerId => _answers.First(answer => answer.IsCorrect).Id;

    public static Result<Question> Create(
        Guid id,
        Guid categoryId,
        QuestionText text,
        IReadOnlyCollection<AnswerCandidate> answers)
    {
        ArgumentNullException.ThrowIfNull(answers);

        if (categoryId == Guid.Empty)
        {
            return Failure<Question>(QuestionErrors.EmptyCategory);
        }

        if (answers.Count != RequiredAnswers)
        {
            return Failure<Question>(QuestionErrors.InvalidAnswerCount);
        }

        if (answers.Count(candidate => candidate.IsCorrect) != 1)
        {
            return Failure<Question>(QuestionErrors.InvalidCorrectAnswerCount);
        }

        if (answers.Select(candidate => candidate.Text).Distinct().Count() != RequiredAnswers)
        {
            return Failure<Question>(QuestionErrors.DuplicateAnswerText);
        }

        Question question = new(id, categoryId, text);
        foreach (AnswerCandidate candidate in answers)
        {
            question._answers.Add(Answer.Create(candidate.Id, id, candidate.Text, candidate.IsCorrect));
        }

        return question;
    }

    public bool IsCorrectAnswer(Guid answerId) =>
        _answers.Any(answer => answer.Id == answerId && answer.IsCorrect);

    public bool HasAnswer(Guid answerId) => _answers.Any(answer => answer.Id == answerId);

    public Result Update(QuestionText text, IReadOnlyCollection<AnswerCandidate> answers)
    {
        ArgumentNullException.ThrowIfNull(answers);

        if (answers.Count != RequiredAnswers)
        {
            return Result.Failure(QuestionErrors.InvalidAnswerCount);
        }

        if (answers.Count(candidate => candidate.IsCorrect) != 1)
        {
            return Result.Failure(QuestionErrors.InvalidCorrectAnswerCount);
        }

        if (answers.Select(candidate => candidate.Text).Distinct().Count() != RequiredAnswers)
        {
            return Result.Failure(QuestionErrors.DuplicateAnswerText);
        }

        Text = text;
        _answers.Clear();
        foreach (AnswerCandidate candidate in answers)
        {
            _answers.Add(Answer.Create(candidate.Id, Id, candidate.Text, candidate.IsCorrect));
        }

        return Success();
    }

    public Result Activate()
    {
        if (IsActive)
        {
            return Result.Failure(QuestionErrors.AlreadyActive);
        }

        IsActive = true;

        return Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
        {
            return Result.Failure(QuestionErrors.AlreadyInactive);
        }

        IsActive = false;

        return Success();
    }
}
