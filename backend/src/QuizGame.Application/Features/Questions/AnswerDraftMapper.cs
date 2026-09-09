using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Questions;

public static class AnswerDraftMapper
{
    public static Result<IReadOnlyCollection<AnswerCandidate>> ToCandidates(
        IReadOnlyCollection<AnswerDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        List<AnswerCandidate> candidates = new(drafts.Count);

        foreach (AnswerDraft draft in drafts)
        {
            Result<AnswerText> text = AnswerText.Create(draft.Text);
            if (text.IsFailure)
            {
                return Result.Failure<IReadOnlyCollection<AnswerCandidate>>(text.Error);
            }

            candidates.Add(new AnswerCandidate(Guid.NewGuid(), text.Value, draft.IsCorrect));
        }

        return Result.Success<IReadOnlyCollection<AnswerCandidate>>(candidates);
    }
}
