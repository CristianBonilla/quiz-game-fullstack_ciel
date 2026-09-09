using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Questions.CreateQuestion;

public sealed record CreateQuestionCommand(
    Guid CategoryId,
    string Text,
    IReadOnlyCollection<AnswerDraft> Answers) : ICommand<QuestionResponse>;
