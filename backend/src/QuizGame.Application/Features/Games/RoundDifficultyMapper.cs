namespace QuizGame.Application.Features.Games;

/// <summary>Maps each round number (1 to 5) to its corresponding difficulty level (1 to 5).</summary>
public static class RoundDifficultyMapper
{
    public static int MapToDifficultyLevel(int roundNumber) => roundNumber switch
    {
        1 => 1,
        2 => 2,
        3 => 3,
        4 => 4,
        _ => 5
    };
}
