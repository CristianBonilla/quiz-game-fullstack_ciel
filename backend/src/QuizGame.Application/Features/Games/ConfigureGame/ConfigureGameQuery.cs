using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.ConfigureGame;

public sealed record ConfigureGameQuery : IQuery<GameConfigurationResponse>;
