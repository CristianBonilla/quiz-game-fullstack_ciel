using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.ConfigureGame;

public sealed class ConfigureGameQueryHandler(
    ICategoryRepository categories,
    IQuestionRepository questions,
    GameSettingsFactory settingsFactory) : IQueryHandler<ConfigureGameQuery, GameConfigurationResponse>
{
    public async Task<Result<GameConfigurationResponse>> HandleAsync(
        ConfigureGameQuery query,
        CancellationToken cancellationToken)
    {
        Result<GameSettings> settings = settingsFactory.Create();
        if (settings.IsFailure)
        {
            return Result.Failure<GameConfigurationResponse>(settings.Error);
        }

        List<RoundConfigurationResponse> rounds = [];

        for (int round = 1; round <= settings.Value.TotalRounds; round++)
        {
            Category? category = await categories
                .GetActiveByDifficultyAsync(RoundDifficultyMapper.MapToDifficultyLevel(round), cancellationToken)
                .ConfigureAwait(false);

            if (category is null)
            {
                return Result.Failure<GameConfigurationResponse>(CategoryErrors.NotFoundForRound(round));
            }

            int available = await questions
                .CountActiveByCategoryAsync(category.Id, cancellationToken)
                .ConfigureAwait(false);

            if (available < Category.MinimumQuestions)
            {
                return Result.Failure<GameConfigurationResponse>(CategoryErrors.NotEnoughQuestions);
            }

            rounds.Add(new RoundConfigurationResponse(
                round,
                category.Id,
                category.Name,
                category.DifficultyLevel.Value,
                category.PrizeAmount.Amount,
                available));
        }

        return new GameConfigurationResponse(
            settings.Value.TotalRounds,
            (int)settings.Value.QuestionTimeLimit.TotalSeconds,
            rounds);
    }
}
