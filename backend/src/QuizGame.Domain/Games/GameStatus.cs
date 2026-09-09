namespace QuizGame.Domain.Games;

public enum GameStatus
{
    NotStarted = 0,
    InProgress = 1,
    Won = 2,
    Lost = 3,
    Withdrawn = 4,
    ForcedEnd = 5
}
