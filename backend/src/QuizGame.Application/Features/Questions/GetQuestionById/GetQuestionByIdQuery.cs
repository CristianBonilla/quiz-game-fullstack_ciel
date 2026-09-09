using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Questions.GetQuestionById;

public sealed record GetQuestionByIdQuery(Guid QuestionId) : IQuery<QuestionResponse>;
