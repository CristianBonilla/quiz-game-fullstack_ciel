using FluentValidation;

namespace QuizGame.Application.Features.Questions.DeactivateQuestion;

public sealed class DeactivateQuestionCommandValidator : AbstractValidator<DeactivateQuestionCommand>
{
    public DeactivateQuestionCommandValidator() => RuleFor(command => command.QuestionId).NotEmpty();
}
