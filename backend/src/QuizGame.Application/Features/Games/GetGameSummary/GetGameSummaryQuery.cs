using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.GetGameSummary;

public sealed record GetGameSummaryQuery(Guid GameId) : IQuery<GameSummaryResponse>;
