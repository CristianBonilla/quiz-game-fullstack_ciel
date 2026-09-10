using QuizGame.Domain.Questions;

namespace QuizGame.Infrastructure.Persistence.Specifications;

public sealed class SelectableQuestionsSpecification : Specification<Question>
{
    public SelectableQuestionsSpecification(Guid categoryId, IReadOnlyCollection<Guid> excludedQuestionIds)
        : base(question => question.CategoryId == categoryId
            && question.IsActive
            && !excludedQuestionIds.Contains(question.Id)) =>
        AddInclude(question => question.Answers);
}
