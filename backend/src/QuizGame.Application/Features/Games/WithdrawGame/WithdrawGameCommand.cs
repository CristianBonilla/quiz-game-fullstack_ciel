using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.WithdrawGame;

public sealed record WithdrawGameCommand(Guid GameId) : ICommand<GameSummaryResponse>;
