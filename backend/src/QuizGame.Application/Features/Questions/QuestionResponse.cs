using QuizGame.Domain.Questions;

namespace QuizGame.Application.Features.Questions;

public sealed record AnswerResponse(Guid Id, string Text, bool IsCorrect);

public sealed record QuestionResponse(
    Guid Id,
    Guid CategoryId,
    string Text,
    bool IsActive,
    IReadOnlyCollection<AnswerResponse> Answers);

public sealed record PlayableAnswerResponse(Guid Id, string Text);

public sealed record PlayableQuestionResponse(
    Guid Id,
    Guid CategoryId,
    string Text,
    IReadOnlyCollection<PlayableAnswerResponse> Answers);

public static class QuestionMappings
{
    public static QuestionResponse ToResponse(this Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        return new QuestionResponse(
            question.Id,
            question.CategoryId,
            question.Text.Value,
            question.IsActive,
            [.. question.Answers.Select(answer => new AnswerResponse(answer.Id, answer.Text.Value, answer.IsCorrect))]);
    }

    /// <summary>Projection served while a round is open: it never carries the correct answer.</summary>
    public static PlayableQuestionResponse ToPlayableResponse(this Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        return new PlayableQuestionResponse(
            question.Id,
            question.CategoryId,
            question.Text.Value,
            [.. question.Answers.Select(answer => new PlayableAnswerResponse(answer.Id, answer.Text.Value))]);
    }
}
