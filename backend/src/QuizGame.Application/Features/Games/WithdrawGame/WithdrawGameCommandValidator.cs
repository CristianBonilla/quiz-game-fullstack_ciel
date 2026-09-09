using FluentValidation;

namespace QuizGame.Application.Features.Games.WithdrawGame;

public sealed class WithdrawGameCommandValidator : AbstractValidator<WithdrawGameCommand>
{
    public WithdrawGameCommandValidator() => RuleFor(command => command.GameId).NotEmpty();
}
