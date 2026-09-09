using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Domain.Games;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Abstractions;

public sealed class GameSettingsFactory(IOptions<GameOptions> options)
{
    private readonly GameOptions _options = options.Value;

    public Result<GameSettings> Create()
    {
        Dictionary<int, Prize> prizeByRound = [];

        for (int round = 1; round <= _options.TotalRounds; round++)
        {
            decimal amount = _options.PrizeByRound.TryGetValue(round, out decimal configured)
                ? configured
                : DefaultPrizeFor(round);

            Result<Prize> prize = Prize.Create(amount);
            if (prize.IsFailure)
            {
                return Result.Failure<GameSettings>(prize.Error);
            }

            prizeByRound[round] = prize.Value;
        }

        return GameSettings.Create(
            _options.TotalRounds,
            TimeSpan.FromSeconds(_options.QuestionTimeLimitSeconds),
            prizeByRound);
    }

    private static decimal DefaultPrizeFor(int round) => 100m * (decimal)Math.Pow(2, round - 1);
}
