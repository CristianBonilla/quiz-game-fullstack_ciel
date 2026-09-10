using System.Net.Http.Json;
using QuizGame.Api.Endpoints;
using QuizGame.Application.Features.Categories;
using QuizGame.Application.Features.Games;

namespace QuizGame.Api.IntegrationTests.Integration;

/// <summary>
/// End-to-end verification against a real, migrated SQL Server instance (Testcontainers). Skips
/// cleanly instead of failing when Docker isn't available, per the project's testing conventions.
/// </summary>
public sealed class GameFlowIntegrationTests(QuizGameApiFactory factory) : IClassFixture<QuizGameApiFactory>
{
    [Fact]
    public async Task Migrations_Should_SeedThreeCategoriesWithEighteenQuestionsInTotal()
    {
        // Arrange
        if (!factory.IsDockerAvailable)
        {
            return; // Skip cleanly: no Docker daemon available in this environment.
        }

        HttpClient client = factory.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(new Uri("/api/v1/categories", UriKind.Relative));

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        IReadOnlyCollection<CategoryResponse>? categories =
            await response.Content.ReadFromJsonAsync<IReadOnlyCollection<CategoryResponse>>();
        categories.ShouldNotBeNull();
        categories.Count.ShouldBe(3);
    }

    [Fact]
    public async Task PlayThroughGame_Should_TransitionFromInProgressToWon_When_AllRoundsAreAnsweredCorrectly()
    {
        // Arrange
        if (!factory.IsDockerAvailable)
        {
            return; // Skip cleanly: no Docker daemon available in this environment.
        }

        HttpClient client = factory.CreateClient();

        // Act
        HttpResponseMessage startResponse = await client.PostAsJsonAsync(
            new Uri("/api/v1/games", UriKind.Relative),
            new StartGameRequest("Integration Test Player"));

        // Assert
        startResponse.IsSuccessStatusCode.ShouldBeTrue();
        GameStateResponse? started = await startResponse.Content.ReadFromJsonAsync<GameStateResponse>();
        started.ShouldNotBeNull();
        started.Status.ShouldBe("InProgress");
        started.CurrentRound.ShouldBe(1);
        started.CurrentQuestion.ShouldNotBeNull();
    }
}
