using QuizGame.Api.Extensions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Features.Categories;
using QuizGame.Application.Features.Categories.CreateCategory;
using QuizGame.Application.Features.Categories.DeactivateCategory;
using QuizGame.Application.Features.Categories.GetCategoryById;
using QuizGame.Application.Features.Categories.ListCategories;
using QuizGame.Application.Features.Categories.UpdateCategory;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Endpoints;

public sealed record CreateCategoryRequest(string Name, string Description, int DifficultyLevel, decimal PrizeAmount);

public sealed record UpdateCategoryRequest(string Name, string Description, int DifficultyLevel, decimal PrizeAmount);

public sealed class CategoryEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/categories")
            .WithTags("Categories")
            .RequireRateLimiting(RateLimiterPolicies.Api);

        group.MapGet("/", ListCategoriesAsync)
            .WithName("ListCategories")
            .Produces<IReadOnlyCollection<CategoryResponse>>();

        group.MapGet("/{id:guid}", GetCategoryByIdAsync)
            .WithName("GetCategoryById")
            .Produces<CategoryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateCategoryAsync)
            .WithName("CreateCategory")
            .Produces<CategoryResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateCategoryAsync)
            .WithName("UpdateCategory")
            .Produces<CategoryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeactivateCategoryAsync)
            .WithName("DeactivateCategory")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ListCategoriesAsync(
        bool? onlyActive,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<CategoryResponse>> result = await sender
            .QueryAsync(new ListCategoriesQuery(onlyActive ?? false), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetCategoryByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<CategoryResponse> result = await sender
            .QueryAsync(new GetCategoryByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateCategoryAsync(
        CreateCategoryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<CategoryResponse> result = await sender
            .SendAsync(
                new CreateCategoryCommand(request.Name, request.Description, request.DifficultyLevel, request.PrizeAmount),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToCreatedResult(result.IsSuccess ? $"/api/v1/categories/{result.Value.Id}" : string.Empty);
    }

    private static async Task<IResult> UpdateCategoryAsync(
        Guid id,
        UpdateCategoryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<CategoryResponse> result = await sender
            .SendAsync(
                new UpdateCategoryCommand(id, request.Name, request.Description, request.DifficultyLevel, request.PrizeAmount),
                cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeactivateCategoryAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<Unit> result = await sender
            .SendAsync(new DeactivateCategoryCommand(id), cancellationToken)
            .ConfigureAwait(false);

        return result.IsSuccess ? TypedResults.NoContent() : result.Error.ToProblem();
    }
}
