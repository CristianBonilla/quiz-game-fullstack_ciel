using QuizGame.Api.Extensions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Features.Questions;
using QuizGame.Application.Features.Questions.CreateQuestion;
using QuizGame.Application.Features.Questions.DeactivateQuestion;
using QuizGame.Application.Features.Questions.GetQuestionById;
using QuizGame.Application.Features.Questions.ListQuestionsByCategory;
using QuizGame.Application.Features.Questions.UpdateQuestion;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Endpoints;

public sealed record AnswerDraftRequest(string Text, bool IsCorrect);

public sealed record CreateQuestionRequest(Guid CategoryId, string Text, IReadOnlyCollection<AnswerDraftRequest> Answers);

public sealed record UpdateQuestionRequest(string Text, IReadOnlyCollection<AnswerDraftRequest> Answers);

public sealed class QuestionEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/questions")
            .WithTags("Questions")
            .RequireRateLimiting(RateLimiterPolicies.Api);

        group.MapGet("/{id:guid}", GetQuestionByIdAsync)
            .WithName("GetQuestionById")
            .Produces<QuestionResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateQuestionAsync)
            .WithName("CreateQuestion")
            .Produces<QuestionResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateQuestionAsync)
            .WithName("UpdateQuestion")
            .Produces<QuestionResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeactivateQuestionAsync)
            .WithName("DeactivateQuestion")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGroup("/api/v1/categories")
            .WithTags("Questions")
            .RequireRateLimiting(RateLimiterPolicies.Api)
            .MapGet("/{categoryId:guid}/questions", ListQuestionsByCategoryAsync)
            .WithName("ListQuestionsByCategory")
            .Produces<IReadOnlyCollection<QuestionResponse>>();
    }

    private static async Task<IResult> GetQuestionByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<QuestionResponse> result = await sender
            .QueryAsync(new GetQuestionByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateQuestionAsync(
        CreateQuestionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<QuestionResponse> result = await sender
            .SendAsync(
                new CreateQuestionCommand(request.CategoryId, request.Text, ToAnswerDrafts(request.Answers)),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToCreatedResult(result.IsSuccess ? $"/api/v1/questions/{result.Value.Id}" : string.Empty);
    }

    private static async Task<IResult> UpdateQuestionAsync(
        Guid id,
        UpdateQuestionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<QuestionResponse> result = await sender
            .SendAsync(new UpdateQuestionCommand(id, request.Text, ToAnswerDrafts(request.Answers)), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeactivateQuestionAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<Unit> result = await sender
            .SendAsync(new DeactivateQuestionCommand(id), cancellationToken)
            .ConfigureAwait(false);

        return result.IsSuccess ? TypedResults.NoContent() : result.Error.ToProblem();
    }

    private static async Task<IResult> ListQuestionsByCategoryAsync(
        Guid categoryId,
        bool? onlyActive,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<QuestionResponse>> result = await sender
            .QueryAsync(new ListQuestionsByCategoryQuery(categoryId, onlyActive ?? false), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static IReadOnlyCollection<AnswerDraft> ToAnswerDrafts(IReadOnlyCollection<AnswerDraftRequest> answers) =>
        [.. answers.Select(answer => new AnswerDraft(answer.Text, answer.IsCorrect))];
}
