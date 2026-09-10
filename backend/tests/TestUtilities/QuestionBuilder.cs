using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.TestUtilities;

public sealed class QuestionBuilder
{
    private readonly List<(Guid Id, string Text, bool IsCorrect)> _answers = [];

    private Guid _id = Guid.NewGuid();
    private Guid _categoryId = Guid.NewGuid();
    private string _text = "Sample question?";

    public static QuestionBuilder AQuestion() => new();

    public QuestionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public QuestionBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public QuestionBuilder WithText(string text)
    {
        _text = text;
        return this;
    }

    public QuestionBuilder WithFourAnswers()
    {
        _answers.Clear();
        _answers.Add((Guid.NewGuid(), "Option A", false));
        _answers.Add((Guid.NewGuid(), "Option B", false));
        _answers.Add((Guid.NewGuid(), "Option C", false));
        _answers.Add((Guid.NewGuid(), "Option D", false));
        return this;
    }

    public QuestionBuilder WithCorrectAnswerAt(int index, Guid? answerId = null)
    {
        if (_answers.Count == 0)
        {
            WithFourAnswers();
        }

        for (int i = 0; i < _answers.Count; i++)
        {
            (Guid answerIdAtIndex, string answerText, bool _) = _answers[i];
            _answers[i] = i == index
                ? (answerId ?? answerIdAtIndex, answerText, true)
                : (answerIdAtIndex, answerText, false);
        }

        return this;
    }

    public QuestionBuilder WithAnswers(params (string Text, bool IsCorrect)[] answers)
    {
        _answers.Clear();
        foreach ((string text, bool isCorrect) in answers)
        {
            _answers.Add((Guid.NewGuid(), text, isCorrect));
        }

        return this;
    }

    public Question Build()
    {
        if (_answers.Count == 0)
        {
            WithCorrectAnswerAt(0);
        }

        List<AnswerCandidate> candidates = [.. _answers
            .Select(answer => new AnswerCandidate(answer.Id, AnswerText.Create(answer.Text).Value, answer.IsCorrect))];

        return Question.Create(_id, _categoryId, QuestionText.Create(_text).Value, candidates).Value;
    }
}
