using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Strategies;

public interface IQuestionSelectionStrategy
{
    Task<Result<Question>> SelectAsync(QuestionSelectionContext context, CancellationToken cancellationToken);
}
