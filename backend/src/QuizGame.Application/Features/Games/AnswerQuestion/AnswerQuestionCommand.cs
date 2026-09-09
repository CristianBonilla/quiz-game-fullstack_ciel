using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.AnswerQuestion;

public sealed record AnswerQuestionCommand(Guid GameId, Guid AnswerId, Guid RequestId)
    : ICommand<AnswerQuestionResponse>, IIdempotentCommand;
