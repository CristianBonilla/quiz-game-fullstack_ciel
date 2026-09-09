using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Questions;

public static class QuestionErrors
{
    public static readonly Error InvalidAnswerCount = Error.Validation(
        "Question.InvalidAnswerCount",
        $"A question must have exactly {Question.RequiredAnswers} answers.");

    public static readonly Error InvalidCorrectAnswerCount = Error.Validation(
        "Question.InvalidCorrectAnswerCount",
        "A question must have exactly one correct answer.");

    public static readonly Error DuplicateAnswerText = Error.Validation(
        "Question.DuplicateAnswerText",
        "A question cannot have two answers with the same text.");

    public static readonly Error EmptyCategory = Error.Validation(
        "Question.EmptyCategory",
        "A question must belong to a category.");

    public static readonly Error AlreadyInactive = Error.Conflict(
        "Question.AlreadyInactive",
        "The question is already inactive.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "Question.AlreadyActive",
        "The question is already active.");

    public static Error NotFound(Guid questionId) => Error.NotFound(
        "Question.NotFound",
        $"The question with identifier '{questionId}' was not found.");

    public static Error NoneAvailable(Guid categoryId) => Error.NotFound(
        "Question.NoneAvailable",
        $"No unused active question is available for category '{categoryId}'.");
}
