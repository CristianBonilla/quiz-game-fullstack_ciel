using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Questions.ListQuestionsByCategory;

public sealed record ListQuestionsByCategoryQuery(Guid CategoryId, bool OnlyActive)
    : IQuery<IReadOnlyCollection<QuestionResponse>>;
