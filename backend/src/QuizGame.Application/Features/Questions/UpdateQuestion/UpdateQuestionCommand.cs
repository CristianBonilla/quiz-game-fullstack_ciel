using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Questions.UpdateQuestion;

public sealed record UpdateQuestionCommand(
    Guid QuestionId,
    string Text,
    IReadOnlyCollection<AnswerDraft> Answers) : ICommand<QuestionResponse>;
