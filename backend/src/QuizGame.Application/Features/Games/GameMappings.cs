using QuizGame.Application.Features.Questions;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;

namespace QuizGame.Application.Features.Games;

public static class GameMappings
{
    public static GameStateResponse ToStateResponse(this Game game, Question? currentQuestion)
    {
        ArgumentNullException.ThrowIfNull(game);

        Round? openRound = game.Rounds.FirstOrDefault(round => !round.IsAnswered);

        return new GameStateResponse(
            game.Id,
            game.PlayerName.Value,
            game.Status.ToString(),
            game.CurrentRound.Value,
            game.Settings.TotalRounds,
            game.AccumulatedPrize.Amount,
            openRound?.PrizeAtStake.Amount ?? 0m,
            currentQuestion?.ToPlayableResponse(),
            openRound?.DeadlineUtc);
    }

    public static GameSummaryResponse ToSummaryResponse(this Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        return new GameSummaryResponse(
            game.Id,
            game.PlayerName.Value,
            game.Status.ToString(),
            game.AccumulatedPrize.Amount,
            game.Rounds.Count,
            game.StartedAtUtc,
            game.EndedAtUtc,
            [.. game.Rounds
                .OrderBy(round => round.Number.Value)
                .Select(round => new RoundSummaryResponse(
                    round.Number.Value,
                    round.QuestionId,
                    round.Outcome.ToString(),
                    round.PrizeAtStake.Amount,
                    round.SelectedAnswerId,
                    round.AnsweredAtUtc))]);
    }
}
