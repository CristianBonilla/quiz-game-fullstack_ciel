using FluentValidation;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Questions.CreateQuestion;

public sealed class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(command => command.CategoryId).NotEmpty();
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
