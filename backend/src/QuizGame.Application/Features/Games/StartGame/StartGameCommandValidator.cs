using FluentValidation;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Games.StartGame;

public sealed class StartGameCommandValidator : AbstractValidator<StartGameCommand>
{
    public StartGameCommandValidator() =>
        RuleFor(command => command.PlayerName).NotEmpty().MaximumLength(PlayerName.MaxLength);
}
