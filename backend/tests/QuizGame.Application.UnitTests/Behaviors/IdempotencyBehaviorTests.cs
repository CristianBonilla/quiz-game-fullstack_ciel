using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Observability;
using QuizGame.Application.Behaviors;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.AnswerQuestion;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.UnitTests.Behaviors;

public sealed class IdempotencyBehaviorTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    [Fact]
    public async Task HandleAsync_Should_InvokeContinuationOnlyOnce_When_RequestIdIsNew()
    {
        // Arrange
        IIdempotencyStore store = Substitute.For<IIdempotencyStore>();
        store.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<IdempotentRequest?>(null));

        IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse> behavior = new(
            store,
            new IdempotencyContext(),
            new ResilienceMetrics(),
            Substitute.For<ILogger<IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse>>>());

        AnswerQuestionCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        AnswerQuestionResponse response = ASuccessfulResponse();
        int continuationCalls = 0;
        NextHandler<AnswerQuestionResponse> continuation = () =>
        {
            continuationCalls++;
            return Task.FromResult(Result.Success(response));
        };

        // Act
        Result<AnswerQuestionResponse> result = await behavior.HandleAsync(command, continuation, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        continuationCalls.ShouldBe(1);
    }

    [Fact]
    public async Task HandleAsync_Should_ReplayStoredResponse_When_SameRequestIdIsSentTwice()
    {
        // Arrange
        AnswerQuestionResponse storedResponse = ASuccessfulResponse();
        Guid requestId = Guid.NewGuid();

        IdempotentRequest storedRequest = new(
            requestId,
            nameof(AnswerQuestionCommand),
            storedResponse.GameId,
            JsonSerializer.Serialize(storedResponse, SerializerOptions),
            200,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        IIdempotencyStore store = Substitute.For<IIdempotencyStore>();
        store.FindAsync(requestId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<IdempotentRequest?>(storedRequest));

        IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse> behavior = new(
            store,
            new IdempotencyContext(),
            new ResilienceMetrics(),
            Substitute.For<ILogger<IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse>>>());

        AnswerQuestionCommand command = new(storedResponse.GameId, Guid.NewGuid(), requestId);
        int continuationCalls = 0;
        NextHandler<AnswerQuestionResponse> continuation = () =>
        {
            continuationCalls++;
            return Task.FromResult(Result.Success(storedResponse));
        };

        // Act
        Result<AnswerQuestionResponse> result = await behavior.HandleAsync(command, continuation, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameId.ShouldBe(storedResponse.GameId);
        continuationCalls.ShouldBe(0);
    }

    [Fact]
    public async Task HandleAsync_Should_ReplayWinnerResponse_When_ConcurrentDuplicateKeyViolationOccurs()
    {
        // Arrange
        AnswerQuestionResponse winnerResponse = ASuccessfulResponse();
        Guid requestId = Guid.NewGuid();

        IdempotentRequest winnerRecord = new(
            requestId,
            nameof(AnswerQuestionCommand),
            winnerResponse.GameId,
            JsonSerializer.Serialize(winnerResponse, SerializerOptions),
            200,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1));

        IIdempotencyStore store = Substitute.For<IIdempotencyStore>();
        store.FindAsync(requestId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IdempotentRequest?>(null), Task.FromResult<IdempotentRequest?>(winnerRecord));
        store.IsDuplicateKeyViolation(Arg.Any<InvalidOperationException>()).Returns(true);

        IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse> behavior = new(
            store,
            new IdempotencyContext(),
            new ResilienceMetrics(),
            Substitute.For<ILogger<IdempotencyBehavior<AnswerQuestionCommand, AnswerQuestionResponse>>>());

        AnswerQuestionCommand command = new(winnerResponse.GameId, Guid.NewGuid(), requestId);
        NextHandler<AnswerQuestionResponse> continuation = () =>
            throw new InvalidOperationException("Simulated unique-index violation from a concurrent winner.");

        // Act
        Result<AnswerQuestionResponse> result = await behavior.HandleAsync(command, continuation, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.GameId.ShouldBe(winnerResponse.GameId);
    }

    [Fact]
    public async Task HandleAsync_Should_BypassIdempotency_When_RequestIsNotIdempotentCommand()
    {
        // Arrange
        IIdempotencyStore store = Substitute.For<IIdempotencyStore>();

        IdempotencyBehavior<NonIdempotentPing, string> behavior = new(
            store,
            new IdempotencyContext(),
            new ResilienceMetrics(),
            Substitute.For<ILogger<IdempotencyBehavior<NonIdempotentPing, string>>>());

        int continuationCalls = 0;
        NextHandler<string> continuation = () =>
        {
            continuationCalls++;
            return Task.FromResult(Result.Success("pong"));
        };

        // Act
        Result<string> result = await behavior.HandleAsync(new NonIdempotentPing(), continuation, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        continuationCalls.ShouldBe(1);
        await store.DidNotReceive().FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    private static AnswerQuestionResponse ASuccessfulResponse() => new(
        Guid.NewGuid(),
        1,
        true,
        Guid.NewGuid(),
        "InProgress",
        100m,
        2,
        200m,
        null,
        null);

}

/// <summary>Must be public: NSubstitute needs a public type to build the ILogger proxy for the generic behavior.</summary>
public sealed record NonIdempotentPing;
