using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.GetGameState;

public sealed record GetGameStateQuery(Guid GameId) : IQuery<GameStateResponse>;
