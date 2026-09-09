using FluentValidation;

namespace QuizGame.Application.Features.Games.ForceEndGame;

public sealed class ForceEndGameCommandValidator : AbstractValidator<ForceEndGameCommand>
{
    public ForceEndGameCommandValidator()
    {
        RuleFor(command => command.GameId).NotEmpty();
        RuleFor(command => command.Reason).NotEmpty().MaximumLength(200);
    }
}
