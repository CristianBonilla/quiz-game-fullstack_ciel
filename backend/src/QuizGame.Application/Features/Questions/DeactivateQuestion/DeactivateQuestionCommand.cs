using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Questions.DeactivateQuestion;

public sealed record DeactivateQuestionCommand(Guid QuestionId) : ICommand;
