using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Games.StartGame;

public sealed record StartGameCommand(string PlayerName) : ICommand<GameStateResponse>;
