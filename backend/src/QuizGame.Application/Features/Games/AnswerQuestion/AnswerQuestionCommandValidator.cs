using FluentValidation;

namespace QuizGame.Application.Features.Games.AnswerQuestion;

public sealed class AnswerQuestionCommandValidator : AbstractValidator<AnswerQuestionCommand>
{
    public AnswerQuestionCommandValidator()
    {
        RuleFor(command => command.GameId).NotEmpty();
        RuleFor(command => command.AnswerId).NotEmpty();
        RuleFor(command => command.RequestId).NotEmpty();
    }
}
