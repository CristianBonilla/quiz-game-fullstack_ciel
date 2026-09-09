using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.ForceEndGame;

public sealed record ForceEndGameCommand(Guid GameId, string Reason) : ICommand<GameSummaryResponse>;
