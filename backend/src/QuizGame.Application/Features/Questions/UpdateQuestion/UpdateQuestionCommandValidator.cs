using FluentValidation;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Questions.UpdateQuestion;

public sealed class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(command => command.QuestionId).NotEmpty();
        RuleFor(command => command.Text).NotEmpty().MaximumLength(QuestionText.MaxLength);
        RuleFor(command => command.Answers)
            .NotNull()
            .Must(answers => answers.Count == Question.RequiredAnswers)
            .WithMessage($"A question must have exactly {Question.RequiredAnswers} answers.");
        RuleFor(command => command.Answers)
            .Must(answers => answers.Count(answer => answer.IsCorrect) == 1)
            .WithMessage("A question must have exactly one correct answer.")
            .When(command => command.Answers is not null);
        RuleForEach(command => command.Answers)
            .ChildRules(answer => answer.RuleFor(draft => draft.Text)
                .NotEmpty()
                .MaximumLength(AnswerText.MaxLength));
    }
}
