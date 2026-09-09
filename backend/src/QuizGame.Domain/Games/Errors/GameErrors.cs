using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Errors;

public static class GameErrors
{
    public static readonly Error NotInProgress = Error.Conflict(
        "Game.NotInProgress",
        "The game is not in progress.");

    public static readonly Error RoundAlreadyOpen = Error.Conflict(
        "Game.RoundAlreadyOpen",
        "The current round already has a question assigned and is still unanswered.");

    public static readonly Error RoundAlreadyAnswered = Error.Conflict(
        "Game.RoundAlreadyAnswered",
        "The current round has already been answered.");

    public static readonly Error NoOpenRound = Error.Conflict(
        "Game.NoOpenRound",
        "There is no open round to act upon.");

    public static readonly Error CannotWithdrawAfterAnswering = Error.Conflict(
        "Game.CannotWithdrawAfterAnswering",
        "The player cannot withdraw after answering the current round.");

    public static readonly Error QuestionAlreadyAsked = Error.Conflict(
        "Game.QuestionAlreadyAsked",
        "The question was already used in this game.");

    public static readonly Error QuestionNotActive = Error.Validation(
        "Game.QuestionNotActive",
        "An inactive question cannot be assigned to a round.");

    public static readonly Error RoundExpired = Error.Conflict(
        "Game.RoundExpired",
        "The answer arrived after the round deadline.");

    public static readonly Error AnswerNotInQuestion = Error.Validation(
        "Game.AnswerNotInQuestion",
        "The selected answer does not belong to the current question.");

    public static readonly Error EmptyForcedEndReason = Error.Validation(
        "Game.EmptyForcedEndReason",
        "A forced end requires a reason.");

    public static Error NotFound(Guid gameId) => Error.NotFound(
        "Game.NotFound",
        $"The game with identifier '{gameId}' was not found.");

    public static Error InvalidTransition(GameStatus from, GameStatus to) => Error.Conflict(
        "Game.InvalidTransition",
        $"A game cannot transition from '{from}' to '{to}'.");
}
