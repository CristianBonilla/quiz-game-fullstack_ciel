namespace QuizGame.Application.Features.Games;

/// <summary>Rounds 1-2 draw from the easiest category, 3-4 from the intermediate one, 5 from the hardest.</summary>
public static class RoundDifficultyMapper
{
    public static int MapToDifficultyLevel(int roundNumber) => roundNumber switch
    {
        1 or 2 => 1,
        3 or 4 => 2,
        _ => 3
    };
}
